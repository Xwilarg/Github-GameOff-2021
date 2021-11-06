using UnityEngine;

namespace Bug.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        private GameObject _target;
        private void Start()
        {
            _target = transform.GetChild(0).gameObject;
        }

        public void Toggle()
        {
            _target.SetActive(!_target.activeInHierarchy);
        }

        public bool IsActive()
        {
            return _target.activeInHierarchy;
        }
    }
}
