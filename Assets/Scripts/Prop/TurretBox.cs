using Bug.Player;
using UnityEngine;

namespace Bug.Prop
{
    public class TurretBox : MonoBehaviour
    {
        public void Pick(PlayerController pc)
        {
            Destroy(gameObject);
        }
    }
}
