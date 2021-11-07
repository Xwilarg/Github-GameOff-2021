using System;

namespace Bug
{
	public static class GameStateManager
	{
		private static bool _paused;

		public static bool Paused {
			get => _paused;
			set {
				if (_paused != value)
				{
					_paused = value;
					OnPauseStateChanged?.Invoke(_paused);
				}
			}
		}

		public static event Action<bool> OnPauseStateChanged;
	}
}
