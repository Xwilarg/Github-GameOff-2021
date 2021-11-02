using UnityEngine;

namespace Bug.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/RoomInfo", fileName = "RoomInfo")]
    public class RoomInfo : ScriptableObject
    {
        public Vector2Int Size;

        public bool HaveSouthDoor;
        public bool HaveNorthDoor;
        public bool HaveEastDoor;
        public bool HaveWestDoor;
    }
}
