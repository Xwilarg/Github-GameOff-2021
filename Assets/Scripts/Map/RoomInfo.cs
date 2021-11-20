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
    }
}
