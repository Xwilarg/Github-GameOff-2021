using System.Collections;
using Bug.Enemy;
using Bug.Player;
using TMPro;
using UnityEngine;

namespace Bug.WeaponSystem
{
	public class HitscanFirearm : Firearm
	{
		[SerializeField] private LayerMask _shootingLayerMask = -1;

		[SerializeField] private Transform _tip;

		[SerializeField] private GameObject _muzzleEffect;
		[SerializeField] private GameObject _impactEffect;

		[SerializeField] private GameObject _damageDisplay;


		protected override void Shoot(int _)
		{
			if (_muzzleEffect != null)
			{
				GameObject effect = Instantiate(_muzzleEffect, _tip);
				Destroy(effect, effect.TryGetComponent(out ParticleSystem particles) ? particles.main.duration : 2f);
			}

			if (Player != null)
			{
				Camera camera = Player.Camera;

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
							go.transform.LookAt(Player.transform.position, Vector3.up);
							go.transform.rotation = Quaternion.Euler(0f, go.transform.rotation.eulerAngles.y + 180, 0f);
						}
					}
				}
			}
		}

		public override void SecondaryActionBegin() { }

		public override void SecondaryActionEnd() { }
	}
}
