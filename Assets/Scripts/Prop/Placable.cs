using UnityEngine;

namespace Bug.Prop
{
    public class Placable : MonoBehaviour
    {
        [SerializeField]
        private Collider _detectionCollider;

        [SerializeField]
        private Transform _groundPoint;

        public Transform GroundPoint => _groundPoint;

        public bool IsOverlappingObjects()
        {
            if (_detectionCollider.Raycast(new(_detectionCollider.transform.position, Vector3.forward), out RaycastHit hit, .1f))
            {
                Debug.Log(hit.collider.name);
                return true;
            }
            return false;
        }
    }
}
