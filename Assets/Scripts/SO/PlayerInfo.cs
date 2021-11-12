using UnityEngine;

namespace Bug.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerInfo", fileName = "PlayerInfo")]
    public class PlayerInfo : ScriptableObject
    {
        [Tooltip("Number of magazines in starting weapon")]
        public int NbOfMagazine;
        [Tooltip("Weapon the player starts with")]
        public WeaponInfo MainWeapon;
        [Tooltip("Speed multiplicator when the player is running")]
        public float SpeedRunningMultiplicator;
        [Tooltip("Jump force of the player")]
        public float JumpForce;
        [Tooltip("Local gravity multiplicator for jump")]
        public float GravityMultiplicator;
    }
}
