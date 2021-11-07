using UnityEngine;
using UnityEngine.Events;

namespace Bug.Prop
{
    public class Pickable : MonoBehaviour
    {
        public UnityEvent OnPlace { get; } = new();
    }
}
