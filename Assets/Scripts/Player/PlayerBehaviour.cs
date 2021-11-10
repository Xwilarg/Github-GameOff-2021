using UnityEngine;

namespace Bug.Player
{
	public class PlayerBehaviour : MonoBehaviour
	{
		public int Index { get; set; }

		public IPlayerController Controller => GetComponent<IPlayerController>();


		private void Awake()
		{
			PlayerManager.AddPlayer(this);
		}

		private void OnDestroy()
		{
			PlayerManager.RemovePlayer(this);
		}
	}
}
