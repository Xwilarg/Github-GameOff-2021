using System.Collections;
using UnityEngine;

namespace Bug.WeaponSystem
{
	public abstract class FireArm : MonoBehaviour, IWeapon
	{
		[SerializeField] private int _ammoCapacity;
		[SerializeField] private float _reloadDuration;
		[SerializeField] private float _fireRate = 5f;
		[SerializeField] private int _animationIndex;

		public abstract bool CanHandlePrimaryAction { get; }

		public abstract bool CanHandleSecondaryAction { get; }

		public abstract bool CanReload { get; }

		public AmmoData Ammo { get; private set; }


		protected virtual void Awake()
		{
			Ammo = new AmmoData(_ammoCapacity, 0);
		}

		public abstract IEnumerator HandlePrimaryAction();

		public abstract IEnumerator HandleSecondaryAction();

		public abstract IEnumerator HandleReload();
	}
}
