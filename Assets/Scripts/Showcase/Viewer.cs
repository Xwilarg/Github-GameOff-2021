using TMPro;
using UnityEngine;

namespace Bug.Showcase
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField]
        private AssetData[] _assets;

        [SerializeField]
        private TMP_Text _madeBy;

        private int _currentIndex;
        private GameObject _currentInstance;

        private void Start()
        {
            DisplayAsset();
        }

        private void Update()
        {
            // TODO: Use input system
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q)) // Thinking of our french friends...
            {
                _currentIndex--;
                if (_currentIndex == -1)
                {
                    _currentIndex = _assets.Length - 1;
                }
                DisplayAsset();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                _currentIndex++;
                if (_currentIndex == _assets.Length)
                {
                    _currentIndex = 0;
                }
                DisplayAsset();
            }
            else if (Input.GetKeyDown(KeyCode.R)) // Reset position / rotation
            {
                ResetTransform();
            }
        }

        private void ResetTransform()
        {
            _currentInstance.transform.position = Vector3.zero;
            _currentInstance.transform.rotation = Quaternion.identity;
        }

        private void DisplayAsset()
        {
            if (_currentInstance != null)
            {
                Destroy(_currentInstance);
            }
            _currentInstance = Instantiate(_assets[_currentIndex].Prefab);
            _madeBy.text = "Asset made by " + _assets[_currentIndex].Author.ToString();
        }

        public void Rotate(Vector2 dir)
        {
            _currentInstance.transform.Rotate(dir.y, dir.x, 0f);
        }

        public void Move(Vector3 dir)
        {
            _currentInstance.transform.position += dir;
        }
    }
}
