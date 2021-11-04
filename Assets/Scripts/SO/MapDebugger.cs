using UnityEngine;

namespace Bug.Map
{
    public class MapDebugger : MonoBehaviour
    {
        public static MapDebugger S;

        private void Awake()
        {
            S = this;
        }

        public int DebugId { private set; get; }

        public void SetDebugId(int id)
        {
            DebugId = id;
        }
    }
}