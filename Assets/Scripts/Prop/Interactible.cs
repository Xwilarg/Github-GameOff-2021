using Bug.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Bug.Prop
{
    public class Interactible : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<PlayerController> _activated;

        [SerializeField]
        private UnityEvent<Vector3> _onHover;

        [SerializeField]
        private UnityEvent _onHoverLeave;

        public void AddListenerOnActivated(UnityAction<PlayerController> act)
        {
            _activated.AddListener(act);
        }

        public void Activate(PlayerController pc)
        {
            _activated?.Invoke(pc);
        }

        public void Hover(Vector3 pos)
        {
            _onHover?.Invoke(pos);
        }

        public void HoverLeave()
        {
            _onHoverLeave?.Invoke();
        }
    }
}
