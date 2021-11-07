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
            SetActive(!IsActive());
        }

        public void SetActive(bool active)
        {
            _target.SetActive(active);
        }

        public bool IsActive()
        {
            return _target.activeInHierarchy;
        }
    }
}
