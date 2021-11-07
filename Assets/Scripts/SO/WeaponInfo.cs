using Bug.Weapon;
using UnityEngine;

namespace Bug.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/WeaponInfo", fileName = "WeaponInfo")]
    public class WeaponInfo : ScriptableObject
    {
        public string Name;
        public int MaxNbOfBullets;
        public float ReloadDuration;
        public WeaponType Type;
    }
}
