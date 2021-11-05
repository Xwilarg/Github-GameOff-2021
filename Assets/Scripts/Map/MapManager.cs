using Bug.SO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bug.Map
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField]
        private MapInfo _mapInfo;

        [SerializeField]
        private RoomInfo[] _availableRooms;

        [SerializeField]
        private RoomInfo _startingRoom;

        [SerializeField]
        private string _seed;

        [SerializeField]
        private GameObject _playerPrefab;

        private readonly List<Room> _currentRooms = new();
        private GameObject _roomContainer;

        private void Start()
        {
            _roomContainer = new("Rooms");

            // Add starting room
            var start = CreateFromRoomInfo(Vector2Int.zero, _startingRoom, RoomType.STARTING, _mapInfo.MaxPathLength);
            Instantiate(_playerPrefab, new Vector3(start.Position.x + start.Size.x / 2f, 2f, start.Position.y + start.Size.y / 2f), Quaternion.identity);
            _currentRooms.Add(start);

            // Init seed
            Random.InitState(_seed.GetHashCode());

            // Add all rooms
            if (_startingRoom.HaveSouthDoor) AddRoom(Vector2Int.zero, start, _mapInfo.MaxPathLength, Vector2Int.down);
            if (_startingRoom.HaveNorthDoor) AddRoom(Vector2Int.zero, start, _mapInfo.MaxPathLength, Vector2Int.up);
            if (_startingRoom.HaveEastDoor) AddRoom(Vector2Int.zero, start, _mapInfo.MaxPathLength, Vector2Int.right);
            if (_startingRoom.HaveWestDoor) AddRoom(Vector2Int.zero, start, _mapInfo.MaxPathLength, Vector2Int.left);

            // Set zones
            DrawZones();

            // Reset seed
            Random.InitState((int)System.DateTime.Now.Ticks);
        }

        private void AddRoom(Vector2 lastPosition, Room lastRoom, int remainingIteration, Vector2Int direction)
        {
            if (remainingIteration > 0)
            {
                // Get all rooms that can fit
                var available = _availableRooms
                    .Where(x => // Make sure there is a door matching where we want to go
                    {
                        return
                            (direction == Vector2Int.up && x.HaveSouthDoor) ||
                            (direction == Vector2Int.down && x.HaveNorthDoor) ||
                            (direction == Vector2Int.left && x.HaveEastDoor) ||
                            (direction == Vector2Int.right && x.HaveWestDoor);
                    })
                    .Select(x =>
                    {
                        // Let's say we have a 3x3 room called R and we want to add a new room called N
                        // We start by taking the position of the room R
                        // - If our direction go in the negative, we need to substract the size of the room N to place it properly
                        // - If our direction go in the position, we need to add the size of the room R to place it next to it
                        int xOffset = 0, yOffset = 0;
                        if (direction.x > 0) xOffset = lastRoom.Size.x;
                        else if (direction.x < 0) xOffset = -x.Size.x;
                        if (direction.y > 0) yOffset = lastRoom.Size.y;
                        else if (direction.y < 0) yOffset = -x.Size.y;
                        var currPos = lastPosition + new Vector2(xOffset, yOffset);

                        // So now we need to offset the doors so everything match
                        // If our placement direction if left, that means we need to go down/up (etc)
                        // Then we calculate the offset depending of the 2 room sizes
                        var additionalOffset = new Vector2Int(
                            Mathf.Abs(direction.x) == 1 ? 0 : 1,
                            Mathf.Abs(direction.y) == 1 ? 0 : 1
                        ) * new Vector2Int(
                            lastRoom.Size.x / 2 - x.Size.x / 2,
                            lastRoom.Size.y / 2 - x.Size.y / 2
                        );

                        currPos += additionalOffset;

                        return (x, currPos);
                    })
                    .Where(x => // https://stackoverflow.com/a/306332/6663248
                    {
                        return !_currentRooms.Any(r =>
                        {
                            return x.currPos.x < (r.Position.x + r.Size.x) && (x.currPos.x + x.x.Size.x) > r.Position.x &&
                                x.currPos.y < (r.Position.y + r.Size.y) && (x.currPos.y + x.x.Size.y) > r.Position.y;
                        });
                    })
                    .ToArray();

                if (available.Length == 0) // No room available
                {
                    return;
                }
                var ri = available[Random.Range(0, available.Length)];

                // Create room and set variables
                var r = CreateFromRoomInfo(ri.currPos, ri.x, remainingIteration > _mapInfo.UnlockedRange ? RoomType.AVAILABLE : RoomType.LOCKED, remainingIteration - 1);
                if (direction == Vector2Int.up)
                {
                    lastRoom.Up = r;
                    r.Down = lastRoom;
                }
                if (direction == Vector2Int.down)
                {
                    lastRoom.Down = r;
                    r.Up = lastRoom;
                }
                if (direction == Vector2Int.left)
                {
                    lastRoom.Left = r;
                    r.Right = lastRoom;
                }
                if (direction == Vector2Int.right)
                {
                    lastRoom.Right = r;
                    r.Left = lastRoom;
                }
                _currentRooms.Add(r);

                // Create child rooms
                if (ri.x.HaveSouthDoor && direction != Vector2Int.down) AddRoom(ri.currPos, r, remainingIteration - 1, Vector2Int.down);
                if (ri.x.HaveNorthDoor && direction != Vector2Int.up) AddRoom(ri.currPos, r, remainingIteration - 1, Vector2Int.up);
                if (ri.x.HaveEastDoor && direction != Vector2Int.left) AddRoom(ri.currPos, r, remainingIteration - 1, Vector2Int.right);
                if (ri.x.HaveWestDoor && direction != Vector2Int.right) AddRoom(ri.currPos, r, remainingIteration - 1, Vector2Int.left);
            }
        }

        private void DrawZones()
        {
            List<Room> _endRooms;
            List<Room> _nextRooms // Take all the rooms that were generated last
                = _currentRooms
                .Where(x => x.Distance == 0 && x.Type != RoomType.AVAILABLE)
                .ToList();
            int id = 1;
            foreach (var r in _nextRooms)
            {
                r.ZoneId = id++;
            }

            while (_nextRooms.Any())
            {
                _endRooms = new(_nextRooms);
                _nextRooms.Clear();
                foreach (var r in _endRooms)
                {
                    List<Room> rooms = new()
                    {
                        r.Up, r.Down, r.Left, r.Right
                    };
                    // Remove rooms we don't want
                    // If 2 rooms have a strictly positive ID, we merge them with the biggest
                    rooms.RemoveAll(x => x == null || x.Type != RoomType.LOCKED || (x.ZoneId == 0 || (x.ZoneId > 0 && x.ZoneId >= r.ZoneId)));
                    foreach (var nr in rooms)
                    {
                        nr.ZoneId = r.ZoneId;
                    }
                    _nextRooms.AddRange(rooms);
                }
            }

            // We have a lot of IDs but since some were merge, lot of numbers are missing between them
            // So we clean that a bit
            id = 1;
            Dictionary<int, int> ids = new();
            foreach (var room in _currentRooms)
            {
                if (room.ZoneId > 0)
                {
                    if (!ids.ContainsKey(room.ZoneId))
                    {
                        var tmp = id++;
                        ids.Add(room.ZoneId, tmp);
                        room.ZoneId = tmp;
                    }
                    else
                    {
                        room.ZoneId = ids[room.ZoneId];
                    }
                }
            }
        }

        private Room CreateFromRoomInfo(Vector2 position, RoomInfo info, RoomType type, int distance)
        {
            var go = Instantiate(info.gameObject, new Vector3(position.x + info.Size.x / 2f, 0f, position.y + info.Size.y / 2f), Quaternion.identity);
            go.transform.parent = _roomContainer.transform;
            return new(info.Size, position, type, info, go, distance);
        }

        private void OnDrawGizmos()
        {
            int id = MapDebugger.S?.DebugId ?? 0;
            if (id == 1)
            {
                #region limits
                foreach (var r in _currentRooms.OrderBy(x =>
                {
                    return x.Type switch
                    {
                        RoomType.STARTING => 1,
                        RoomType.AVAILABLE => 0,
                        RoomType.LOCKED => -1,
                        _ => throw new System.NotImplementedException($"Invalid type {x.Type}")
                    };
                }))
                {
                    // Draw room
                    Gizmos.color = r.Type switch
                    {
                        RoomType.STARTING => Color.green,
                        RoomType.AVAILABLE => Color.white,
                        RoomType.LOCKED => Color.grey,
                        _ => throw new System.NotImplementedException($"Invalid type {r.Type}")
                    };
                    Vector3 pos = new(r.Position.x, 0f, r.Position.y);
                    Vector3 size = new(r.Size.x, 0f, r.Size.y);
                    Gizmos.DrawLine(pos, pos + Vector3.right * size.x);
                    Gizmos.DrawLine(pos, pos + Vector3.forward * size.z);
                    Gizmos.DrawLine(pos + size, pos + Vector3.right * size.x);
                    Gizmos.DrawLine(pos + size, pos + Vector3.forward * size.z);
                    Gizmos.DrawLine(pos, pos + size);
                    Gizmos.DrawLine(pos + Vector3.right * r.Size.x, pos + Vector3.forward * r.Size.y);

                    // Draw door
                    // Red doors are the ones that doesn't lead anywhere, the others are blue
                    var colorOk = new Color(0f, 0f, 1f, .2f);
                    var colorNo = new Color(1f, 0f, 0f, .2f);
                    var center = pos + size / 2f + Vector3.up * 1f;
                    if (r.Info.HaveSouthDoor)
                    {
                        if (r.Down == null) Gizmos.color = colorNo;
                        else Gizmos.color = colorOk;
                        Gizmos.DrawCube(center + size.z * Vector3.back / 2f, new Vector3(1f, 2f, .3f));
                    }
                    if (r.Info.HaveNorthDoor)
                    {
                        if (r.Up == null) Gizmos.color = colorNo;
                        else Gizmos.color = colorOk;
                        Gizmos.DrawCube(center + size.z * Vector3.forward / 2f, new Vector3(1f, 2f, .3f));
                    }
                    if (r.Info.HaveEastDoor)
                    {
                        if (r.Right == null) Gizmos.color = colorNo;
                        else Gizmos.color = colorOk;
                        Gizmos.DrawCube(center + size.x * Vector3.right / 2f, new Vector3(.3f, 2f, 1f));
                    }
                    if (r.Info.HaveWestDoor)
                    {
                        if (r.Left == null) Gizmos.color = colorNo;
                        else Gizmos.color = colorOk;
                        Gizmos.DrawCube(center + size.x * Vector3.left / 2f, new Vector3(.3f, 2f, 1f));
                    }
                }
                #endregion
            }
            else if (id == 2)
            {
                #region zones
                var colors = new[]
                {
                    Color.red, Color.blue, Color.green,
                    Color.magenta, Color.cyan, Color.yellow
                };
                foreach (var room in _currentRooms)
                {
                    Gizmos.color = room.ZoneId switch
                    {
                        0 => Color.white, // Starting zone
                        -1 => Color.black, // Not supposed to happen
                        _ => colors[(room.ZoneId - 1) % colors.Length]
                    };
                    Gizmos.DrawCube(new Vector3(room.Position.x + room.Size.x / 2f, 2f, room.Position.y + room.Size.y / 2f), new Vector3(room.Size.x, 4f, room.Size.y));
                }
                #endregion
            }
            else if (id == 3)
            {
                #region distance
                foreach (var room in _currentRooms)
                {
                    var force = room.Distance * 1f / _mapInfo.MaxPathLength;
                    Gizmos.color = new Color(1f - force, force, 0f);
                    Gizmos.DrawCube(new Vector3(room.Position.x + room.Size.x / 2f, 2f, room.Position.y + room.Size.y / 2f), new Vector3(room.Size.x, 4f, room.Size.y));
                }
                #endregion
            }
        }
    }
}