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

        private readonly List<Room> _currentRooms = new();

        private void Start()
        {
            // Add starting room and generate everything from there
            _currentRooms.Add(CreateFromRoomInfo(Vector2Int.zero, _startingRoom));
            if (_startingRoom.HaveSouthDoor) AddRoom(Vector2Int.zero, _startingRoom, _mapInfo.MaxPathLength, Vector2Int.down);
            else if (_startingRoom.HaveNorthDoor) AddRoom(Vector2Int.zero, _startingRoom, _mapInfo.MaxPathLength, Vector2Int.up);
            else if (_startingRoom.HaveEastDoor) AddRoom(Vector2Int.zero, _startingRoom, _mapInfo.MaxPathLength, Vector2Int.right);
            else if (_startingRoom.HaveWestDoor) AddRoom(Vector2Int.zero, _startingRoom, _mapInfo.MaxPathLength, Vector2Int.left);
        }

        private void AddRoom(Vector2 lastPosition, RoomInfo lastRoom, int remainingIteration, Vector2Int direction)
        {
            if (remainingIteration > 0)
            {
                var available = _availableRooms
                    .Select(x =>
                    {
                        // Let's say we have a 3x3 room called R and we want to add a new room called N
                        // We start by taking the position of the room R
                        // - If our direction go in the negative, we need to substract the size of the room N to place it properly
                        // - If our direction go in the position, we need to add the size of the room R to place it next to it
                        var currPos = lastPosition
                            + new Vector2(direction.x > 0 ? lastRoom.Size.x : -x.Size.x, direction.y > 0 ? lastRoom.Size.y : -x.Size.y);

                        // So now we need to offset the doors so everything match
                        // If our placement direction if left, that means we need to go down/up (etc)
                        // Then we calculate the offset depending of the 2 room sizes
                        var additionalOffset = new Vector2Int(
                            Mathf.Abs(direction.x) == 1 ? 0 : 1,
                            Mathf.Abs(direction.y) == 1 ? 0 : 1
                        ) * new Vector2Int(
                            (lastRoom.Size.x / 2) - (x.Size.x),
                            (lastRoom.Size.y / 2) - (x.Size.y)
                        );

                        currPos += additionalOffset;

                        return (x, currPos);
                    })
                    .Where(x => // https://stackoverflow.com/a/306332/6663248
                    {
                        return !_currentRooms.Any(r =>
                        {
                            return x.currPos.x < (r.Position.x + r.Size.x) && (x.currPos.x + x.x.Size.x) > r.Position.x &&
                                x.currPos.y > (x.currPos.y + x.x.Size.y) && (x.currPos.y + x.x.Size.y) < r.Position.y;
                        });
                    })
                    .ToArray();

                if (available.Length == 0) // No room available
                {
                    return;
                }
                var ri = available[Random.Range(0, available.Length)];

                _currentRooms.Add(CreateFromRoomInfo(ri.currPos, ri.x));
                if (ri.x.HaveSouthDoor) AddRoom(ri.currPos, ri.x, remainingIteration - 1, Vector2Int.down);
                else if (ri.x.HaveNorthDoor) AddRoom(ri.currPos, ri.x, remainingIteration - 1, Vector2Int.up);
                else if (ri.x.HaveEastDoor) AddRoom(ri.currPos, ri.x, remainingIteration - 1, Vector2Int.right);
                else if (ri.x.HaveWestDoor) AddRoom(ri.currPos, ri.x, remainingIteration - 1, Vector2Int.left);
            }
        }

        private Room CreateFromRoomInfo(Vector2 position, RoomInfo info)
        {
            return new(info.Size, position);
        }

        private void OnDrawGizmos()
        {
            foreach (var r in _currentRooms)
            {
                Vector3 pos = new(r.Position.x, 0f, r.Position.y);
                Gizmos.DrawLine(pos, pos + Vector3.right * r.Size.x);
                Gizmos.DrawLine(pos, pos + Vector3.forward * r.Size.y);
                Gizmos.DrawLine(pos + new Vector3(r.Size.x, 0f, r.Size.y), pos + Vector3.right * r.Size.x);
                Gizmos.DrawLine(pos + new Vector3(r.Size.x, 0f, r.Size.y), pos + Vector3.forward * r.Size.y);
                Gizmos.DrawLine(pos, pos + new Vector3(r.Size.x, 0f, r.Size.y));
                Gizmos.DrawLine(pos + Vector3.right * r.Size.x, pos + Vector3.forward * r.Size.y);
            }
        }
    }
}