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

        private float _health = 100f;

        public int TakeDamage(Collider collider, int damage)
        {
            var baseHealth = _health;
            float finalDamage;
            if (_internalModifier.ContainsKey(collider))
            {
                finalDamage = damage * _internalModifier[collider];
            }
            else
            {
                finalDamage = damage;
            }
            _health -= finalDamage;
            if (_health <= 0)
            {
                Destroy(gameObject);
            }
            return Mathf.FloorToInt(baseHealth - _health); // Because of rounding, we can't just rely on damages done
        }
    }
}
