using System;
using UnityEngine;

namespace Bug.Player
{
	public class PlayerAnimationEventsBroadcaster : MonoBehaviour
	{
		public event Action OnReloaded;


		public void ReceivedAnimReloaded()
		{
			OnReloaded?.Invoke();
		}
	}
}
