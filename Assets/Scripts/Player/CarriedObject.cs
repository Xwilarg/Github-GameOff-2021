using Bug.Prop;
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

        private GameObject _hint;
        private GameObject _prefab;
        private Placable _placeInfo;
    }
}
