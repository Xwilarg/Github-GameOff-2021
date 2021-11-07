using UnityEngine;
using Random = UnityEngine.Random;

namespace Bug
{
	public class TestSpawner : MonoBehaviour
	{
		[Range(0f, 1f)]
		[SerializeField] private float _probability = 0.5f;

		[SerializeField] private GameObject[] _prefabs;


		private void Start()
		{
			if (Random.value <= _probability)
			{
				Spawn();
			}
		}

		public void Spawn()
		{
			if (_prefabs == null || _prefabs.Length == 0)
				return;

			int index = Random.Range(0, _prefabs.Length);
			GameObject prefab = _prefabs[index];

			Instantiate(prefab, transform.position, Quaternion.identity);
		}
	}
}
