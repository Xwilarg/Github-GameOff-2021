using UnityEngine;

namespace Bug
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleSystemDestroyAfterDuration : MonoBehaviour
	{
		private ParticleSystem _particleSystem;


		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
		}

		private void Start()
		{
			ParticleSystem.MainModule main = _particleSystem.main;
			main.loop = false;
			Destroy(gameObject, main.duration);
		}
	}

}
