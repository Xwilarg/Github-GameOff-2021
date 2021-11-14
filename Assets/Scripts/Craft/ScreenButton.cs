using UnityEngine;
using UnityEngine.Events;

namespace Bug.Craft
{
    public class ScreenButton : MonoBehaviour
    {
        public UnityEvent OnHoverEnter { get; } = new();
        public UnityEvent OnHoverExit { get; } = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pointer"))
            {
                OnHoverEnter?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Pointer"))
            {
                OnHoverExit?.Invoke();
            }
        }
    }
}
