using System.Collections.Generic;
using Bug.Menu;
using TMPro;
using UnityEngine;

namespace Bug.Player
{
	[DefaultExecutionOrder(-100)]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager S;

        public PauseMenu PauseMenu;
        public GameObject PressE;
        public TMP_Text AmmoDisplay;

        public List<PlayerBehaviour> AllPlayers { get; } = new List<PlayerBehaviour>();


        private void Awake()
        {
            S = this;
        }

        public PlayerBehaviour GetPlayer(int index = 0)
        {
	        if (index < AllPlayers.Count - 1)
		        return null;

	        return AllPlayers[index];
        }

        public void AddPlayer(PlayerBehaviour playerBehaviour)
        {
	        if (!AllPlayers.Contains(playerBehaviour))
		        AllPlayers.Add(playerBehaviour);
        }

        public void RemovePlayer(PlayerBehaviour playerBehaviour)
        {
	        if (AllPlayers.Contains(playerBehaviour))
		        AllPlayers.Remove(playerBehaviour);
        }
    }
}
