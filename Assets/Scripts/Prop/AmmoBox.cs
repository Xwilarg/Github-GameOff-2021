using Bug.Player;
using UnityEngine;

namespace Bug.Prop
{
    public class AmmoBox : MonoBehaviour
    {
        public void EarnAmmo(PlayerController pc)
        {
            Destroy(gameObject);
        }
    }
}
