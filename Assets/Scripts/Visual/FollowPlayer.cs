using UnityEngine;

namespace Bug.Visual
{
    public class FollowPlayer : MonoBehaviour
    {
        public Transform Target { set; private get; }

        private void Update()
        {
            if (Target != null)
            {
                transform.position = new Vector3(Target.position.x, transform.position.y, Target.position.z);
            }
        }
    }
}
