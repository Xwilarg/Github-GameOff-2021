using UnityEngine;

namespace Bug.Craft
{
    public class Screen : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pointer;

        private GameObject _pointerInstance;

        public void Hover(Vector3 pos)
        {
            if (_pointerInstance == null)
            {
                _pointerInstance = Instantiate(_pointer, pos, Quaternion.Euler(0f, 0f, 90f));
                _pointerInstance.transform.parent = transform;
            }
            else
            {
                _pointerInstance.transform.position = pos;
            }
        }

        public void HoverEnd()
        {
            Destroy(_pointerInstance);
            _pointerInstance = null;
        }
    }
}
