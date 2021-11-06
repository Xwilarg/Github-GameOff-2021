using Bug.Menu;
using TMPro;
using UnityEngine;

namespace Bug.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager S;

        private void Awake()
        {
            S = this;
        }

        public PauseMenu PauseMenu;
        public GameObject PressE;
        public TMP_Text AmmoDisplay;
    }
}
