using UnityEngine;

namespace Bug.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerInfo", fileName = "PlayerInfo")]
    public class PlayerInfo : ScriptableObject
    {
        public int NbOfMagazine;
        public WeaponInfo _mainWeapon;
    }
}
