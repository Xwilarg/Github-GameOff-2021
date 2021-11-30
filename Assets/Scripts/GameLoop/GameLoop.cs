using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bug
{
	public class GameLoop : MonoBehaviour
	{
		public static GameLoop Current { get; private set; }


		private void Awake()
		{
			Current = this;
		}

		private void Start()
		{
			GameStateManager.Paused = false;
		}

		public void Restart()
		{
			SceneManager.LoadSceneAsync("Loader");
		}
	}
}
