using UnityEngine;
using UnityEngine.EventSystems;

namespace Bug.Showcase
{
    public class UIHandler : MonoBehaviour, IDragHandler, IScrollHandler
    {
        [SerializeField]
        private Viewer _viewer;

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _viewer.Rotate(eventData.delta);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                _viewer.Move(eventData.delta * .01f);
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            _viewer.Move(Vector3.back * eventData.scrollDelta.y * .25f);
        }
    }
}
