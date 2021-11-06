﻿using UnityEngine;

namespace Bug.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/WeaponInfo", fileName = "WeaponInfo")]
    public class WeaponInfo : ScriptableObject
    {
        public string Name;
        public int MaxNbOfBullets;
    }
}
