using Bug.SO;
using System.Collections.Generic;
using UnityEngine;

namespace Bug.Map
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField]
        private MapInfo _mapInfo;

        [SerializeField]
        private RoomInfo[] _rooms;

        [SerializeField]
        private RoomInfo _startingRoom; // Let's say thing this is 2x2 with a door at its south

        private readonly List<Room> _currentRooms = new();

        private void Start()
        {
            _currentRooms.Add(CreateFromRoomInfo(Vector2Int.zero, _startingRoom));
            AddRoom(_mapInfo.MaxPathLength, Vector2Int.up * 2);
        }

        private void AddRoom(int remainingIteration, Vector2Int nextPosition)
        {
            if (remainingIteration > 0)
            {
                var ri = _rooms[0]; // TODO: take a random room
                //AddRoom(remainingIteration - 1);
            }
        }

        private Room CreateFromRoomInfo(Vector2Int position, RoomInfo info)
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
            }
        }
    }
}