using UnityEngine;

namespace Bug.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu S;
        private GameObject _target;

        private void Awake()
        {
            S = this;
        }

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
