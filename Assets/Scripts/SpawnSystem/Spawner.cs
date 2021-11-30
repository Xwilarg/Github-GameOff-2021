using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bug.SpawnSystem
{
	public class Spawner : MonoBehaviour
	{
		[SerializeField] private GameObject[] _spawnPrefabs;
		[SerializeField] private float _spawnCooldown = 60f;
		[Range(0f, 1f)]
		[SerializeField] private float _probability = 0.8f;

		private GameObject _instance;
		private float _lastSpawnTime;


		private void Start()
		{
			_lastSpawnTime = -_spawnCooldown;
		}

		public void SpawnIfPossible()
		{
			if (_instance == null && Time.time > _lastSpawnTime + _spawnCooldown)
			{
				Spawn();
			}
		}

		public void Spawn()
		{
			if (Random.value <= _probability)
			{
				GameObject prefab = GetRandomPrefab();
				_instance = Instantiate(prefab, transform.position, Quaternion.identity);
			}
			_lastSpawnTime = Time.time;
		}

		private GameObject GetRandomPrefab()
		{
			return _spawnPrefabs[Random.Range(0, _spawnPrefabs.Length)];
		}
	}
}
