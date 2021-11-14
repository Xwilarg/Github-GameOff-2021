using System.Collections;
using Bug.Player;
using UnityEngine;

namespace Bug.WeaponSystem
{
	public abstract class Firearm : MonoBehaviour, IHoldable, IPrimaryActionHandler, ISecondaryActionHandler, IReloadHandler
	{
		[SerializeField] private int _animationIndex; //TODO: Branch animation tree based on weapon animation index - Antoine F. 12/11/2021

		[Header("Ammo, Reload")]
		[SerializeField] private float _reloadDuration;
		[SerializeField] private int _ammoCapacity;
		[SerializeField] private bool _fillAmmoOnStart;

		[Header("Shooting")]
		[SerializeField] private float _fireRate = 5f;
		[SerializeField] private ShootingMode _shootingMode = ShootingMode.Burst;
		[SerializeField] private int _burstCount = 1;

		[Header("Audio")]
		[SerializeField] private AudioSource _audioSource;
		[SerializeField] private AudioClip _audioClipShoot;
		[SerializeField] private AudioClip _audioClipReload;

		public GameObject Holder { get; set; }
		protected PlayerController Player { get; set; }

		public int BurstCount { get => _burstCount; set => _burstCount = value; }
		public bool TriggerPulled { get; protected set; }
		public bool Shooting { get; protected set; }

		public int AmmoCapacity { get => _ammoCapacity; set => _ammoCapacity = value; }
		public int AmmoCount { get; set; }

		public bool Reloading { get; protected set; }

		private Coroutine _reloadCoroutine;

		private static readonly int _animParamShoot = Animator.StringToHash("Shooting");
		private static readonly int _animParamAiming = Animator.StringToHash("Aiming");
		private static readonly int _animParamReload = Animator.StringToHash("Reload");


		protected virtual void Start()
		{
			if (_fillAmmoOnStart)
				AmmoCount = AmmoCapacity;
		}

		#region Holding

		public GameObject GetHolder() => Holder;

		public void HoldBegin(GameObject holder)
		{
			Holder = holder;
			if (holder != null)
			{
				Player = holder.GetComponent<PlayerController>();
			}
		}

		public void HoldEnd(GameObject holder)
		{
			if (Holder == holder)
			{
				Holder = null;
				Player = null;
			}
		}

		#endregion Holding

		#region Primary Action

		protected abstract void Shoot(int bulletInBurstIndex);

		public virtual void PrimaryActionBegin()
		{
			if (!TriggerPulled && !Reloading)
			{
				TriggerPulled = true;
				if (!Shooting)
				{
					StartCoroutine(ShootingCoroutine());
				}
			}
		}

		public virtual void PrimaryActionEnd()
		{
			TriggerPulled = false;
		}

		protected virtual IEnumerator ShootingCoroutine()
		{
			if (AmmoCount <= 0) yield break;

			Shooting = true;
			SetAnimatorBool(_animParamShoot, true);

			int currentBurst = 0;
			float timeBetweenShots = 1f / Mathf.Max(_fireRate, Mathf.Epsilon);

			while (_shootingMode == ShootingMode.Burst && currentBurst < BurstCount || _shootingMode == ShootingMode.Continuous && TriggerPulled)
			{
				Shoot(currentBurst++);

				PlaySound(_audioClipShoot);

				AmmoCount--;
				if (AmmoCount <= 0) break;

				yield return new WaitForSeconds(timeBetweenShots);
			}

			SetAnimatorBool(_animParamShoot, false);
			Shooting = false;
		}

		#endregion Primary Action

		#region Reload

		public virtual void ReloadRequested()
		{
			if (!Reloading)
			{
				_reloadCoroutine = StartCoroutine(ReloadCoroutine_Internal());
			}
		}

		public void ReloadCancel()
		{
			if (Reloading)
			{
				StopCoroutine(_reloadCoroutine);
				Reloading = false;
			}
		}

		private IEnumerator ReloadCoroutine_Internal()
		{
			Reloading = true;
			yield return StartCoroutine(ReloadCoroutine());
			Reloading = false;
		}

		protected virtual IEnumerator ReloadCoroutine()
		{
			if (Player != null && Player.Animator != null)
			{
				SetAnimatorTrigger(_animParamReload);
				PlaySound(_audioClipReload);
			}
			yield return new WaitForSeconds(_reloadDuration);
			AmmoCount = AmmoCapacity;
		}

		#endregion Reload

		protected virtual void PlaySound(AudioClip clip)
		{
			if (clip != null && _audioSource != null)
			{
				_audioSource.clip = clip;
				_audioSource.Play();
			}
		}

		protected void SetAnimatorTrigger(int property)
		{
			if (Player != null && Player.Animator != null)
			{
				Player.Animator.SetTrigger(property);
			}
		}

		protected void SetAnimatorBool(int property, bool state)
		{
			if (Player != null && Player.Animator != null)
			{
				Player.Animator.SetBool(property, state);
			}
		}

		public virtual void SecondaryActionBegin()
		{
			SetAnimatorBool(_animParamAiming, true);
		}

		public virtual void SecondaryActionEnd()
		{
			SetAnimatorBool(_animParamAiming, false);
		}
	}
}
