using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Bug.Map
{
    public class RoomInfo : MonoBehaviour
    {
        public Vector2Int Size;

        public bool HaveSouthDoor;
        public bool HaveNorthDoor;
        public bool HaveEastDoor;
        public bool HaveWestDoor;

        public RoomType Type;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2, new Vector3(Size.x, 4, Size.y));

            Gizmos.color = Color.red;

            if (HaveSouthDoor)
                Gizmos.DrawWireCube(transform.position + Vector3.up * 1.5f + new Vector3(0, 0, -Size.y * 0.5f), new Vector3(1, 3, 0.2f));
            if (HaveNorthDoor)
                Gizmos.DrawWireCube(transform.position + Vector3.up * 1.5f + new Vector3(0, 0, Size.y * 0.5f), new Vector3(1, 3, 0.2f));
            if (HaveEastDoor)
                Gizmos.DrawWireCube(transform.position + Vector3.up * 1.5f + new Vector3(Size.x * 0.5f, 0, 0), new Vector3(0.2f, 3, 1));
            if (HaveWestDoor)
                Gizmos.DrawWireCube(transform.position + Vector3.up * 1.5f + new Vector3(-Size.x * 0.5f, 0, 0), new Vector3(0.2f, 3, 1));
        }
    }
}
