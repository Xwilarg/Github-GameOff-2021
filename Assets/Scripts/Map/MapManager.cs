using System;
using Bug.SO;
using Bug.Visual;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bug.Objective;
using Random = UnityEngine.Random;

namespace Bug.Map
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Information about the map")]
        private MapInfo _mapInfo;

        [SerializeField]
        [Tooltip("Rooms that will be used for the generation")]
        private RoomInfo[] _availableRooms;

        [SerializeField]
        [Tooltip("Rooom the player will start in")]
        private RoomInfo _startingRoom;

        [SerializeField]
        [Tooltip("Rooms containing objectives")]
        private RoomInfo _objectiveRoom;

        [SerializeField]
        [Tooltip("Seed used for the generation")]
        private string _seed;

        [SerializeField]
        [Tooltip("Player prefab to spawn once the map is generated")]
        private GameObject _playerPrefab;

        [SerializeField]
        [Tooltip("Camera script that make it to follow the player (used for the minimap)")]
        private FollowPlayer _follow;

        [SerializeField]
        [Tooltip("Prefab for enemy spawners")]
        private GameObject _spawner;

        [SerializeField]
        [Tooltip("Prefab for doors")]
        private GameObject _doorPrefab;

        [SerializeField]
        private MapPostProcessor[] _postProcessors;

        private GameProgression _progression;

        public List<Room> AllRooms { get; } = new();
        private GameObject _roomContainer;


        private void Start()
        {
            _roomContainer = new("Rooms");

            // Add starting room and spawn player
            var start = CreateFromRoomInfo(Vector2Int.zero, _startingRoom, RoomState.STARTING, _mapInfo.MaxPathLength);
            var playerIns = Instantiate(_playerPrefab, new Vector3(start.Position.x + start.Size.x / 2f, 2f, start.Position.y + start.Size.y / 2f), Quaternion.identity);
            _follow.Target = playerIns.transform; // Make sure minimap follow player

            AllRooms.Add(start);

            // Init seed
            Random.InitState(_seed.GetHashCode());

            // Add all rooms
            if (_startingRoom.HaveSouthDoor) AddRoom(_availableRooms, Vector2Int.zero, start, 0, Vector2Int.down);
            if (_startingRoom.HaveNorthDoor) AddRoom(_availableRooms, Vector2Int.zero, start, 0, Vector2Int.up);
            if (_startingRoom.HaveEastDoor) AddRoom(_availableRooms, Vector2Int.zero, start, 0, Vector2Int.right);
            if (_startingRoom.HaveWestDoor) AddRoom(_availableRooms, Vector2Int.zero, start, 0, Vector2Int.left);

            var endRooms = AllRooms.Where(x => x.Distance == _mapInfo.MaxPathLength - 1 && x.State != RoomState.AVAILABLE).ToArray();
            var objArr = new[] { _objectiveRoom };
            for (int i = endRooms.Length - 1; i >= 0; i--)
            {
                var c = endRooms[i];
                if (c.Info.HaveSouthDoor) AddRoom(objArr, c.Position, c, _mapInfo.MaxPathLength - 1, Vector2Int.down);
                if (c.Info.HaveNorthDoor) AddRoom(objArr, c.Position, c, _mapInfo.MaxPathLength - 1, Vector2Int.up);
                if (c.Info.HaveEastDoor) AddRoom(objArr, c.Position, c, _mapInfo.MaxPathLength - 1, Vector2Int.right);
                if (c.Info.HaveWestDoor) AddRoom(objArr, c.Position, c, _mapInfo.MaxPathLength - 1, Vector2Int.left);
            }

            // Set zones
            DrawZones();

            // Set doors
            PlaceDoors();

            // Reset seed
            Random.InitState((int)DateTime.Now.Ticks);

            RunPostProcessors();
        }

        private void AddRoom(RoomInfo[] allRooms, Vector2 lastPosition, Room lastRoom, int remainingIteration, Vector2Int direction)
        {
            if (remainingIteration < _mapInfo.MaxPathLength)
            {
                // Get all rooms that can fit
                var available = allRooms
                    .Where(roomInfo => // Make sure there is a door matching where we want to go
                    {
                        return
                            (direction == Vector2Int.up && roomInfo.HaveSouthDoor) ||
                            (direction == Vector2Int.down && roomInfo.HaveNorthDoor) ||
                            (direction == Vector2Int.left && roomInfo.HaveEastDoor) ||
                            (direction == Vector2Int.right && roomInfo.HaveWestDoor);
                    })
                    .Select(roomInfo =>
                    {
                        // Let's say we have a 3x3 room called R and we want to add a new room called N
                        // We start by taking the position of the room R
                        // - If our direction go in the negative, we need to substract the size of the room N to place it properly
                        // - If our direction go in the position, we need to add the size of the room R to place it next to it
                        int xOffset = 0, yOffset = 0;
                        if (direction.x > 0) xOffset = lastRoom.Size.x;
                        else if (direction.x < 0) xOffset = -roomInfo.Size.x;
                        if (direction.y > 0) yOffset = lastRoom.Size.y;
                        else if (direction.y < 0) yOffset = -roomInfo.Size.y;
                        var currPos = lastPosition + new Vector2(xOffset, yOffset);

                        // So now we need to offset the doors so everything match
                        // If our placement direction if left, that means we need to go down/up (etc)
                        // Then we calculate the offset depending of the 2 room sizes
                        var additionalOffset = new Vector2Int(
                            Mathf.Abs(direction.x) == 1 ? 0 : 1,
                            Mathf.Abs(direction.y) == 1 ? 0 : 1
                        ) * new Vector2Int(
                            lastRoom.Size.x / 2 - roomInfo.Size.x / 2,
                            lastRoom.Size.y / 2 - roomInfo.Size.y / 2
                        );

                        currPos += additionalOffset;

                        return (roomInfo, currPos);
                    })
                    .Where(x => // https://stackoverflow.com/a/306332/6663248
                    {
                        return !AllRooms.Any(r =>
                        {
                            return x.currPos.x < (r.Position.x + r.Size.x) && (x.currPos.x + x.roomInfo.Size.x) > r.Position.x &&
                                x.currPos.y < (r.Position.y + r.Size.y) && (x.currPos.y + x.roomInfo.Size.y) > r.Position.y;
                        });
                    })
                    .ToArray();

                if (available.Length == 0) // No room available
                {
                    return;
                }
                var ri = available[Random.Range(0, available.Length)];

                // Create room and set variables
                var r = CreateFromRoomInfo(ri.currPos, ri.roomInfo, remainingIteration < _mapInfo.UnlockedRange ? RoomState.AVAILABLE : RoomState.LOCKED, remainingIteration);
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
                AllRooms.Add(r);

                // Create child rooms
                if (ri.roomInfo.HaveSouthDoor && direction != Vector2Int.down) AddRoom(_availableRooms, ri.currPos, r, remainingIteration + 1, Vector2Int.down);
                if (ri.roomInfo.HaveNorthDoor && direction != Vector2Int.up) AddRoom(_availableRooms, ri.currPos, r, remainingIteration + 1, Vector2Int.up);
                if (ri.roomInfo.HaveEastDoor && direction != Vector2Int.left) AddRoom(_availableRooms, ri.currPos, r, remainingIteration + 1, Vector2Int.right);
                if (ri.roomInfo.HaveWestDoor && direction != Vector2Int.right) AddRoom(_availableRooms, ri.currPos, r, remainingIteration + 1, Vector2Int.left);
            }
        }

        private void DrawZones()
        {
            List<Room> _endRooms;
            List<Room> _nextRooms // Take all the rooms that were generated last
                = AllRooms
                .Where(x => x.Info.Type == RoomType.OBJECTIVE)
                .ToList();
            int id = 1;
            foreach (var r in _nextRooms)
            {
                r.ZoneId = id++;
                var spawner = Instantiate(_spawner, GetRoomCenter(r), Quaternion.identity);
                spawner.transform.parent = r.gameObject.transform;
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
                    rooms.RemoveAll(x => x == null || x.State != RoomState.LOCKED || (x.ZoneId == 0 || (x.ZoneId > 0 && x.ZoneId >= r.ZoneId)));
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
            foreach (var room in AllRooms)
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

            _progression = new(ids.Keys.Count + 1);

            var objPerRoom = _mapInfo.NbObjectives / id - 1;
            var objLeft = _mapInfo.NbObjectives % id - 1;
            int PlaceObjectiveRoom(int id, int count)
            {
                var availableRooms = AllRooms.Where(r => r.Info.Type == RoomType.OBJECTIVE && r.ZoneId == id).ToList();
                while (count > 0 && availableRooms.Count > 0)
                {
                    var rand = Random.Range(0, availableRooms.Count);
                    var room = availableRooms[rand];
                    //var available
                    availableRooms.RemoveAt(rand);
                    count--;
                }
                return count;
            }
            for (int i = 0; i < id - 1; i++)
            {
                var objPlace = objPerRoom;
                if (objLeft > 0)
                {
                    objLeft--;
                    objPlace++;
                }
                objLeft += PlaceObjectiveRoom(i, objPlace);
            }
        }

        private void PlaceDoors()
        {
            foreach (var room in AllRooms)
            {
                if (room.Up == null || (room.State != RoomState.LOCKED && room.Up.State == RoomState.LOCKED))
                {
                    var go = Instantiate(_doorPrefab, new Vector3(room.Position.x + room.Size.x / 2f + 2f, 0f, room.Position.y + room.Size.y - .1f), Quaternion.Euler(0f, 90f, 0f));
                    go.transform.parent = room.transform;
                }
                if (room.Down == null || (room.State != RoomState.LOCKED && room.Down.State == RoomState.LOCKED))
                {
                    var go = Instantiate(_doorPrefab, new Vector3(room.Position.x + room.Size.x / 2f + 2f, 0f, room.Position.y), Quaternion.Euler(0f, 90f, 0f));
                    go.transform.parent = room.transform;
                }
                if (room.Left == null || (room.State != RoomState.LOCKED && room.Left.State == RoomState.LOCKED))
                {
                    var go = Instantiate(_doorPrefab, new Vector3(room.Position.x + .1f, 0f, room.Position.y + room.Size.y / 2f + 2f), Quaternion.identity);
                    go.transform.parent = room.transform;
                }
                if (room.Right == null || (room.State != RoomState.LOCKED && room.Right.State == RoomState.LOCKED))
                {
                    var go = Instantiate(_doorPrefab, new Vector3(room.Position.x + room.Size.x, 0f, room.Position.y + room.Size.y / 2f + 2f), Quaternion.identity);
                    go.transform.parent = room.transform;
                }
            }
        }

        private Room CreateFromRoomInfo(Vector2 position, RoomInfo info, RoomState type, int distance)
        {
            var go = Instantiate(info.gameObject, new Vector3(position.x + info.Size.x / 2f, 0f, position.y + info.Size.y / 2f), Quaternion.identity);
            go.transform.parent = _roomContainer.transform;
            var room = go.AddComponent<Room>();
            room.Configure(info.Size, position, type, info, distance);
            return room;
        }

        private Vector3 GetRoomCenter(Room r)
        {
            return new Vector3(r.Position.x + r.Size.x / 2f, 2f, r.Position.y + r.Size.y / 2f);
        }

        private void RunPostProcessors()
        {
            foreach (MapPostProcessor postProcessor in _postProcessors)
            {
                try
                {
                    postProcessor.Execute(this);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void OnDrawGizmos()
        {
            int id = MapDebugger.S?.DebugId ?? 0;
            if (id == 1)
            {
                #region limits
                foreach (var r in AllRooms.OrderBy(x =>
                {
                    return x.State switch
                    {
                        RoomState.STARTING => 1,
                        RoomState.AVAILABLE => 0,
                        RoomState.LOCKED => -1,
                        _ => throw new System.NotImplementedException($"Invalid type {x.State}")
                    };
                }))
                {
                    // Draw room
                    Gizmos.color = r.State switch
                    {
                        RoomState.STARTING => Color.green,
                        RoomState.AVAILABLE => Color.white,
                        RoomState.LOCKED => Color.grey,
                        _ => throw new System.NotImplementedException($"Invalid type {r.State}")
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
                foreach (var room in AllRooms)
                {
                    Gizmos.color = room.ZoneId switch
                    {
                        0 => Color.white, // Starting zone
                        -1 => Color.black, // Not supposed to happen
                        _ => colors[(room.ZoneId - 1) % colors.Length]
                    };
                    Gizmos.DrawCube(GetRoomCenter(room), new Vector3(room.Size.x, 4f, room.Size.y));
                }
                #endregion
            }
            else if (id == 3)
            {
                #region distance
                foreach (var room in AllRooms)
                {
                    var force = room.Distance * 1f / _mapInfo.MaxPathLength;
                    Gizmos.color = new Color(force, 1f - force, 0f);
                    Gizmos.DrawCube(GetRoomCenter(room), new Vector3(room.Size.x, 4f, room.Size.y));
                }
                #endregion
            }
        }
    }
}
