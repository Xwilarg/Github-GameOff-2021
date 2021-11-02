using UnityEngine;

namespace Bug.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/MapInfo", fileName = "MapInfo")]
    public class MapInfo : ScriptableObject
    {
        [Tooltip("Max number of rooms that can be put next to each other")]
        public int MaxPathLength;
    }
}
