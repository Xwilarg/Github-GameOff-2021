using Bug.Map;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bug
{
	public class SpawnerProcessor : MapPostProcessor
	{
		[SerializeField] private GameObject[] _prefabs;

		public override void Execute(MapManager manager)
		{
			foreach (Room room in manager.AllRooms)
			{
				Spawner spawner = room.GetComponentInChildren<Spawner>();
				if (spawner != null)
				{
					Spawn(spawner.transform.position);
				}
			}
		}

		public void Spawn(Vector3 position)
		{
			if (_prefabs == null || _prefabs.Length == 0)
				return;

			int index = Random.Range(0, _prefabs.Length);
			GameObject prefab = _prefabs[index];

			Instantiate(prefab, position, Quaternion.identity);
		}
	}
}
