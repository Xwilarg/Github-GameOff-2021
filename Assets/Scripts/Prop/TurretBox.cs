using Bug.Player;
using UnityEngine;

namespace Bug.Prop
{
    public class TurretBox : MonoBehaviour
    {
        private Pickable _pickInfo;

        private void Start()
        {
            _pickInfo = GetComponent<Pickable>();
        }

        public void Pick(PlayerController pc)
        {
            pc.PickObject(_pickInfo);
            Destroy(gameObject);
        }
    }
}
