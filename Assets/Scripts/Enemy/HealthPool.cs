using System;
using UnityEngine;

namespace Bug.Enemy
{
	public class HealthPool : MonoBehaviour
	{
		[SerializeField] private bool _damageProof;
		[SerializeField] private float _maxHealth = 100f;
		[SerializeField] private float _currentHealth = 100f;

		public bool DamageProof { get => _damageProof; set => _damageProof = value; }
		public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
		public float CurrentHealth => _currentHealth;

		public event Action OnDepleted;


		public void Refill()
		{
			_currentHealth = _maxHealth;
		}

		public void Modify(float delta)
		{
			if (!_damageProof || delta > 0)
				Set(_currentHealth + delta);
		}

		public void Set(float value)
		{
			if (value < 0f || Mathf.Approximately(0f, value))
				Deplete(true);
			else
				_currentHealth = Mathf.Clamp(value, 0, _maxHealth);
		}

		public void Deplete(bool force = false)
		{
			if (_currentHealth > 0f && (!_damageProof || force))
			{
				_currentHealth = 0f;
				OnDepleted?.Invoke();
			}
		}
	}
}
