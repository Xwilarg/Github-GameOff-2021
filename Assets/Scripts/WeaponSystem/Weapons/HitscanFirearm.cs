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

					GameObject target = hit.rigidbody != null ? hit.rigidbody.gameObject : hit.collider.gameObject;

					if (target.TryGetComponent(out IDamageHandler damageHandler))
					{
						damageHandler.TakeDamage(Damage);
					}
				}
			}
		}
	}
}
