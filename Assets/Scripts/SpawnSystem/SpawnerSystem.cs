using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bug.Map;
using Bug.Player;
using UnityEngine;

namespace Bug.SpawnSystem
{
	public class SpawnerSystem : MapPostProcessor
	{
		private Dictionary<Spawner, Room> _spawners = new Dictionary<Spawner, Room>();

		private Coroutine _currentSpawnRoutine;


		public override void Execute(MapManager manager)
		{
			foreach (Room room in manager.AllRooms)
			{
				Spawner[] spawners = room.GetComponentsInChildren<Spawner>();

				foreach (Spawner spawner in spawners)
				{
					_spawners.Add(spawner, room);
				}
			}

			InvokeRepeating(nameof(TriggerSpawnRoutine), 1f, 1f);
			StartCoroutine(SpawnRoutine(manager.Spawn));
		}

		public void TriggerSpawnRoutine()
		{
			if (_currentSpawnRoutine != null) return;

			PlayerBehaviour player = PlayerManager.GetPlayer();
			if (player == null) return;

			RoomTracker roomTracker = player.GetComponent<RoomTracker>();
			Room currentRoom = roomTracker.CurrentRoom;
			if (currentRoom == null) return;

			_currentSpawnRoutine = StartCoroutine(SpawnRoutine(currentRoom));
		}

		private IEnumerator SpawnRoutine(Room centralRoom)
		{
			List<Room> directNeighbours = GetNeighbours(centralRoom).ToList();
			List<Room> spawnableRooms = new List<Room>();

			foreach (Room room in directNeighbours.SelectMany(GetNeighbours).Where(x => !directNeighbours.Contains(x) && x != centralRoom).Distinct())
			{
				spawnableRooms.Add(room);
				yield return null; // Spread work over multiple frames
			}

			foreach ((Spawner spawner, Room room) in _spawners)
			{
				if (spawnableRooms.Contains(room))
				{
					spawner.SpawnIfPossible();
					Debug.DrawLine(spawner.transform.position, spawner.transform.position + Vector3.up * 4, Color.red, 1f, false);
					yield return null; // Spread work over multiple frames
				}
			}

			_currentSpawnRoutine = null;
		}

		private IEnumerable<Room> GetNeighbours(Room room)
		{
			if (room.Up) yield return room.Up;
			if (room.Down) yield return room.Down;
			if (room.Left) yield return room.Left;
			if (room.Right) yield return room.Right;
		}
	}
}
