using Bug.Player;
using UnityEngine;

namespace Bug.Prop
{
    public class TurretBox : MonoBehaviour
    {
        [SerializeField]
        private GameObject _turretPrefab;

        public void Pick(PlayerController pc)
        {
            pc.PickObject(_turretPrefab);
            Destroy(gameObject);
        }
    }
}
