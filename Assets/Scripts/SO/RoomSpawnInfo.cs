using Bug.Map;
using UnityEngine;

namespace Bug.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/RoomSpawnInfo", fileName = "RoomSpawnInfo")]
    public class RoomSpawnInfo : ScriptableObject
    {
        public RoomInfo RoomInfo;
    }
}
