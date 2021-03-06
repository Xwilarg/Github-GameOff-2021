using System;
using System.Collections.Generic;
using Bug.Menu;
using TMPro;
using UnityEngine;

namespace Bug.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager S;

        public PauseMenu PauseMenu;
        public GameObject PressE;
        public TMP_Text AmmoDisplay;

        public static List<PlayerBehaviour> AllPlayers { get; } = new List<PlayerBehaviour>();


        private void Awake()
        {
            S = this;
            GameStateManager.OnPauseStateChanged += HandleOnPauseStateChanged;
        }

        private void OnDestroy()
        {
            GameStateManager.OnPauseStateChanged -= HandleOnPauseStateChanged;
        }

        private void HandleOnPauseStateChanged(bool paused)
        {
            if (PauseMenu != null)
            {
                PauseMenu.SetActive(paused);
            }
        }

        public static PlayerBehaviour GetPlayer(int index = 0)
        {
            if (AllPlayers.Count > 0 && index < AllPlayers.Count)
                return AllPlayers[index];
            return null;
        }

        public static void AddPlayer(PlayerBehaviour playerBehaviour)
        {
            if (!AllPlayers.Contains(playerBehaviour))
                AllPlayers.Add(playerBehaviour);
        }

        public static void RemovePlayer(PlayerBehaviour playerBehaviour)
        {
            if (AllPlayers.Contains(playerBehaviour))
                AllPlayers.Remove(playerBehaviour);
        }
    }
}
