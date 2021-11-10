using System.Collections;
using Bug.Enemy;
using Bug.Player;
using TMPro;
using UnityEngine;

namespace Bug.WeaponSystem
{
	public class HandCannon : MonoBehaviour, IWeapon
	{
		[SerializeField] private float _fireRate = 2f;

		[SerializeField] private LayerMask _shootingLayerMask = -1;

		[SerializeField] private Transform _tip;

		[SerializeField] private GameObject _muzzleEffect;

		[SerializeField] private GameObject _impactEffect;

		[SerializeField] private GameObject _damageDisplay;

		public bool CanHandlePrimaryAction { get; set; } = true;

		public bool CanHandleSecondaryAction { get; set; } = true;

		public bool CanReload { get; set; } = true;

		private readonly int _shootAnimTrigger = Animator.StringToHash("Shoot");


		public IEnumerator HandlePrimaryAction()
		{
			CanHandlePrimaryAction = false;

			Shoot();

			yield return new WaitForSeconds(1f / _fireRate);
			CanHandlePrimaryAction = true;
		}

		public IEnumerator HandleSecondaryAction()
		{
			yield break;
		}

		public IEnumerator HandleReload()
		{
			yield break;
		}

		public void Shoot()
		{
			if (_muzzleEffect != null)
			{
				GameObject effect = Instantiate(_muzzleEffect, _tip);
				Destroy(effect, effect.TryGetComponent(out ParticleSystem particles) ? particles.main.duration : 2f);
			}

			PlayerController player = GetComponentInParent<PlayerController>();

			if (player != null && player.HeldObject == gameObject)
			{
				Camera camera = player.Camera;

				Ray ray = new(camera.transform.position, camera.transform.forward);

				if (Physics.Raycast(ray, out RaycastHit hit, 100f, _shootingLayerMask))
				{
					if (_impactEffect != null)
					{
						Quaternion rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
						GameObject impact = Instantiate(_impactEffect, hit.point, rotation);
						Destroy(impact, impact.TryGetComponent(out ParticleSystem particles) ? particles.main.duration : 2f);
					}

					var enemy = hit.collider.GetComponentInParent<EnemyData>();
					if (enemy != null) // We hit an ennemy!
					{
						var finalDamage = enemy.TakeDamage(hit.collider, 10);
						if (_damageDisplay != null)
						{
							var go = Instantiate(_damageDisplay, hit.point, Quaternion.identity);
							go.GetComponent<TMP_Text>().text = finalDamage.ToString();
							go.transform.LookAt(player.transform.position, Vector3.up);
							go.transform.rotation = Quaternion.Euler(0f, go.transform.rotation.eulerAngles.y + 180, 0f);
						}
					}
				}

				if (player.Animator != null)
				{
					player.Animator.SetTrigger(_shootAnimTrigger);
				}
			}
		}
	}
}
