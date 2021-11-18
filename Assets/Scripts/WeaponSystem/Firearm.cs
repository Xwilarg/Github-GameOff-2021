using System;
using System.Collections;
using Bug.Player;
using UnityEngine;

namespace Bug.WeaponSystem
{
	public abstract class Firearm : MonoBehaviour, IHoldable, IPrimaryActionHandler, ISecondaryActionHandler, IReloadHandler
	{
		[SerializeField] private int _animationIndex; //TODO: Branch animation tree based on weapon animation index - Antoine F. 12/11/2021

		[Header("Ammo, Reload")]
		[SerializeField] private int _ammoCapacity;
		[SerializeField] private bool _fillAmmoOnStart;

		[Header("Shooting")]
		[SerializeField] private float _fireRate = 5f;
		[SerializeField] private ShootingMode _shootingMode = ShootingMode.Burst;
		[SerializeField] private int _burstCount = 1;
		[SerializeField] private RecoilData _recoil = new(2, 2, 2);
		[SerializeField] private float _aimRecoilFactor = 0.5f;
		[SerializeField] private float _aimZoom = 1.2f;

		[Header("Audio")]
		[SerializeField] private AudioSource _audioSource;
		[SerializeField] private AudioClip _audioClipShoot;
		[SerializeField] private AudioClip _audioClipReload;

		public GameObject Holder { get; set; }
		public Animator Animator { get; private set; }

		public int BurstCount { get => _burstCount; set => _burstCount = value; }
		public bool TriggerPulled { get; protected set; }
		public bool Shooting { get; protected set; }
		public bool Aiming { get; protected set; }

		public int AmmoCapacity { get => _ammoCapacity; set => _ammoCapacity = value; }
		public int AmmoCount { get; set; }
		public bool Reloading { get; protected set; }

		protected PlayerController Player { get; set; }

		private Coroutine _reloadCoroutine;

		private static readonly int _animParamShoot = Animator.StringToHash("Shooting");
		private static readonly int _animParamAiming = Animator.StringToHash("Aiming");
		private static readonly int _animParamReload = Animator.StringToHash("Reload");


		protected void Awake()
		{
			Animator = GetComponent<Animator>();
		}

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
				if (Player != null && Player.AnimationEvents != null)
				{
					Player.AnimationEvents.OnReloaded += HandleOnReloaded;
				}
			}
		}

		public void HoldEnd(GameObject holder)
		{
			if (Holder == holder)
			{
				Holder = null;
				Player = null;
			}

			if (Player != null && Player.AnimationEvents != null)
			{
				Player.AnimationEvents.OnReloaded -= HandleOnReloaded;
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
			SetPlayerAnimatorBool(_animParamShoot, true);

			RecoilMotor recoilMotor = Holder != null ? Holder.GetComponent<RecoilMotor>() : null;

			int currentBurst = 0;
			float timeBetweenShots = 1f / Mathf.Max(_fireRate, Mathf.Epsilon);

			while (_shootingMode == ShootingMode.Burst && currentBurst < BurstCount || _shootingMode == ShootingMode.Continuous && TriggerPulled)
			{
				Shoot(currentBurst++);

				PlaySound(_audioClipShoot);
				PlayAnimation("Shooting");
				if (recoilMotor != null)
					recoilMotor.AddRecoil(_recoil * (Aiming ? _aimRecoilFactor : 1f));

				AmmoCount--;
				if (AmmoCount <= 0) break;

				yield return new WaitForSeconds(timeBetweenShots);
			}

			SetPlayerAnimatorBool(_animParamShoot, false);
			Shooting = false;
		}

		#endregion Primary Action

		#region Secondary Action

		public virtual void SecondaryActionBegin()
		{
			SetPlayerAnimatorBool(_animParamAiming, true);
			Aiming = true;

			if (Holder != null && Holder.TryGetComponent(out CameraAimFOV cameraAimFOV))
				cameraAimFOV.SetZoomAnimated(_aimZoom);
		}

		public virtual void SecondaryActionEnd()
		{
			SetPlayerAnimatorBool(_animParamAiming, false);
			Aiming = false;

			if (Holder != null && Holder.TryGetComponent(out CameraAimFOV cameraAimFOV))
				cameraAimFOV.RestoreZoomAnimated();
		}

		#endregion Secondary Action

		#region Reload

		public virtual void ReloadRequested()
		{
			if (!Reloading && !Shooting && AmmoCount < AmmoCapacity)
			{
				_reloadCoroutine = StartCoroutine(ReloadCoroutine_Internal());
			}
		}

		public bool IsReloading()
		{
			return Reloading;
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
			SetPlayerAnimatorTrigger(_animParamReload);
			PlaySound(_audioClipReload);
			PlayAnimation("Reloading");

			yield return new WaitUntil(() => !Reloading);

			AmmoCount = AmmoCapacity;
		}

		private void HandleOnReloaded()
		{
			Reloading = false;
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

		protected void PlayAnimation(string stateName)
		{
			if (Animator != null)
			{
				Animator.CrossFadeInFixedTime(stateName, 0.05f);
			}
		}

		protected void SetPlayerAnimatorTrigger(int property)
		{
			if (Player != null && Player.Animator != null)
			{
				Player.Animator.SetTrigger(property);
			}
		}

		protected void SetPlayerAnimatorBool(int property, bool state)
		{
			if (Player != null && Player.Animator != null)
			{
				Player.Animator.SetBool(property, state);
			}
		}
	}
}
