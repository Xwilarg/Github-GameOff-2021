using UnityEngine;

namespace Bug.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/MapInfo", fileName = "MapInfo")]
    public class MapInfo : ScriptableObject
    {
        [Tooltip("Max number of rooms that can be put next to each other")]
        public int MaxPathLength;

        [Tooltip("Every rooms before this number will already be unlocked at the start of the game")]
        public int UnlockedRange;

        [Tooltip("Number of objectives the player need to do")]
        public int NbObjectives;
    }
}
