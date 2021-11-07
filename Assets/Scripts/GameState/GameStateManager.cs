using System;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Bug
{
	public static class GameStateManager
	{
		private static bool _paused;
		public static bool Paused { get => _paused; set => SetPaused(value); }
		public static event Action<bool> OnPauseStateChanged;


#if UNITY_EDITOR
		static GameStateManager()
		{
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
		}

		private static void HandlePlayModeStateChanged(PlayModeStateChange stateChange)
		{
			if (stateChange is PlayModeStateChange.ExitingPlayMode or PlayModeStateChange.EnteredPlayMode)
			{
				_paused = false;
			}
		}
#endif // UNITY_EDITOR

		public static void SetPaused(bool paused)
		{
			if (_paused != paused)
			{
				_paused = paused;
				OnPauseStateChanged?.Invoke(_paused);
			}
		}
	}
}
