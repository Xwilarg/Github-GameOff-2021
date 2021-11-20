using Bug.Player;
using Bug.SO;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Bug.Craft
{
    public class Screen : MonoBehaviour
    {
        [SerializeField]
        private RecipeInfo _recipes;

        [SerializeField]
        private GameObject _pointer;

        private GameObject _pointerInstance;

        [SerializeField]
        private ScreenRecipe[] _choices;

        [SerializeField]
        private ScreenButton _previous, _next;

        [SerializeField]
        private GameObject _readyScreen, _waitScreen;

        [SerializeField]
        private Transform _out;

        public UnityEvent<float> StartCrafting { get; } = new();

        private int _index;
        private int _currentRecipe;

        private void Awake()
        {
            _readyScreen.SetActive(true);
            _waitScreen.SetActive(false);
        }

        private void Start()
        {
            foreach (var choice in _choices)
            {
                choice.gameObject.SetActive(false);
            }
            if (_recipes.Recipes.Length <= _choices.Length)
            {
                _previous.gameObject.SetActive(false);
                _next.gameObject.SetActive(false);
            }
            int max = Mathf.Min(_recipes.Recipes.Length, _choices.Length);
            for (int i = 0; i < max; i++)
            {
                var choice = _choices[i];
                var recipe = _recipes.Recipes[i];
                choice.MainText.text = recipe.Name;
                choice.SubText.text = string.Join(" ", recipe.Requirements.Select(x => $"{x.Amount}x {x.Material}"));
                var c = i;
                choice.Button.OnHoverEnter.AddListener(new(() => {
                    choice.MainText.color = Color.red;
                    _currentRecipe = c;
                }));
                choice.Button.OnHoverExit.AddListener(new(() => {
                    choice.MainText.color = Color.black;
                    _currentRecipe = -1;
                }));
                choice.gameObject.SetActive(true);
            }
            // TODO: handle pages
        }

        private IEnumerator Produce(float waitingTime, GameObject obj)
        {
            // Set screen to "Loading..."
            _readyScreen.SetActive(false);
            _waitScreen.SetActive(true);

            StartCrafting?.Invoke(waitingTime);
            yield return new WaitForSeconds(waitingTime); // Craft object...

            Instantiate(obj, _out.position, Quaternion.identity); // Done, instantiate the object

            // Set the screen back to normal
            _readyScreen.SetActive(true);
            _waitScreen.SetActive(false);
        }

        public void Action(PlayerController _)
        {
            if (_currentRecipe != -1)
            {
                var target = _recipes.Recipes[_currentRecipe];
                StartCoroutine(Produce(target.CraftingTime, target.Output));
            }
        }

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

            // Reset hover color
            _currentRecipe = -1;
            foreach (var choice in _choices)
            {
                choice.MainText.color = Color.black;
            }
        }
    }
}
