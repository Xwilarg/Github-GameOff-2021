using System.Collections.Generic;
using UnityEngine;

namespace Bug.Map
{
    public class MapManager : MonoBehaviour
    {
        private readonly List<RoomInfo> _rooms = new();

        private void Start()
        {
            // Create starting room
            CreateRoom();
        }

        private void CreateRoom()
        {
            var room = new RoomInfo();
            _rooms.Add(room);
        }

        private void OnDrawGizmos()
        {
            foreach (var r in _rooms)
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