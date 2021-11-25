using TMPro;
using UnityEngine;

namespace Bug.Craft
{
    [RequireComponent(typeof(ScreenButton))]
    public class ScreenRecipe : MonoBehaviour
    {
        public TMP_Text MainText, SubText;

        public ScreenButton Button => _button;

        private ScreenButton _button;

        private void Awake()
        {
            _button = GetComponent<ScreenButton>();
        }
    }
}
