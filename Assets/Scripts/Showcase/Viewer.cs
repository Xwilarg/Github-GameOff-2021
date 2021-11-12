using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Bug.Showcase
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField]
        private AssetData[] _assets;

        [SerializeField]
        private TMP_Text _madeBy;

        [SerializeField]
        private GameObject _uiPrefab;

        [SerializeField]
        private RectTransform _buttonContainer;

        private int _currentIndex;
        private GameObject _currentInstance;

        private void Start()
        {
            DisplayAsset();

            var ySize = ((RectTransform)_uiPrefab.transform).sizeDelta.y;
            var i = 0;
            foreach (var asset in _assets)
            {
                var go = Instantiate(_uiPrefab, _buttonContainer);
                ((RectTransform)go.transform).anchoredPosition = new(0f, -i * ySize);
                go.GetComponentInChildren<TMP_Text>().text = asset.Name;
                var current = i;
                go.GetComponentInChildren<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    _currentIndex = current;
                    DisplayAsset();
                }));
                i++;
            }
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
