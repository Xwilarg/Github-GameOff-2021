using UnityEngine;

namespace Bug.Prop
{
    public class Placable : MonoBehaviour
    {
        [SerializeField]
        private Collider _detectionCollider;

        public void GetOverlappingObjects()
        {
            if (_detectionCollider.Raycast(new(_detectionCollider.transform.position, Vector3.zero), out RaycastHit hit, 0f))
            {
                Debug.Log(hit.collider.name);
            }
        }
    }
}
