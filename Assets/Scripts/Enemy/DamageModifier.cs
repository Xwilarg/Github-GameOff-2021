using System;
using UnityEngine;

namespace Bug.Enemy
{
    [Serializable]
    public class DamageModifier
    {
        public Collider Collider;

        [Range(0f, 10f)]
        public float Modifier = 1f;
    }
}
