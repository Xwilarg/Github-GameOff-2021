using Bug.Prop;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bug.Player
{
    public class CarriedObject
    {
        public CarriedObject(Transform player, GameObject prefab)
        {
            _prefab = prefab;
            _hint = Object.Instantiate(prefab, player.transform.position + player.transform.forward, player.transform.rotation);
            _placeInfo = _hint.GetComponent<Placable>();
            Assert.IsNotNull(_placeInfo);
        }

        public void UpdatePosition(Transform player)
        {
            _hint.transform.position = player.transform.position + player.transform.forward;
            _hint.transform.rotation = player.transform.rotation;
        }

        public bool CanBePlaced()
        {
            return !_placeInfo.IsOverlappingObjects();
        }

        public void PlaceOnGround()
        {
            if (Physics.Raycast(new Ray(_placeInfo.GroundPoint.position, Vector3.down), out RaycastHit hit))
            {
                _hint.transform.position = new Vector3(_hint.transform.position.x, _hint.transform.position.y - hit.distance, _hint.transform.position.z);
            }
            else
            {
                Debug.LogWarning("Couldn't find ground while placing object...");
            }
        }

        private GameObject _hint;
        private GameObject _prefab;
        private Placable _placeInfo;
    }
}
