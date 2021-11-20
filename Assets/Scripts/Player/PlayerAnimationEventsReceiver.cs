using UnityEngine;

namespace Bug.Player
{
	public class PlayerAnimationEventsReceiver : MonoBehaviour
	{
		[SerializeField] private PlayerAnimationEventsBroadcaster _broadcaster;


		public void AnimReloaded()
		{
			_broadcaster.ReceivedAnimReloaded();
		}
	}
}
