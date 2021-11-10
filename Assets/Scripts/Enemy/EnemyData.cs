using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bug.Enemy
{
    public class EnemyData : MonoBehaviour
    {
        [SerializeField]
        public DamageModifier[] _damageModifier;

        private Dictionary<Collider, float> _internalModifier;

        private void Start()
        {
            _internalModifier = _damageModifier.ToDictionary(x => x.Collider, x => x.Modifier);
        }

        private int _health = 100;

        public int TakeDamage(Collider collider, int damage)
        {
            int finalDamage;
            if (_internalModifier.ContainsKey(collider))
            {
                finalDamage = Mathf.FloorToInt(damage * _internalModifier[collider]);
            }
            else
            {
                finalDamage = damage;
            }
            _health -= finalDamage;
            Destroy(gameObject);
            return finalDamage;
        }
    }
}
