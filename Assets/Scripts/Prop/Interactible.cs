using Bug.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Bug.Prop
{
    public class Interactible : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<PlayerController> _callback;

        public void InvokeCallback(PlayerController pc)
        {
            _callback?.Invoke(pc);
        }
    }
}
