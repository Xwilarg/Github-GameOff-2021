using System;
using Bug.Map;
using UnityEngine;

namespace Bug.Navigation
{
	public class RoomNavMeshConfiguration : MonoBehaviour
	{
		[SerializeField] private GameObject _linkUp;
		[SerializeField] private GameObject _linkDown;
		[SerializeField] private GameObject _linkLeft;
		[SerializeField] private GameObject _linkRight;


		private void Start()
		{
			Room room = GetComponentInParent<Room>();

			DestroyIfUnusable(_linkUp, room.Up);
			DestroyIfUnusable(_linkDown, room.Down);
			DestroyIfUnusable(_linkLeft, room.Left);
			DestroyIfUnusable(_linkRight, room.Right);
		}

		private void DestroyIfUnusable(GameObject link, Room nextRoom)
		{
			if (nextRoom == null)
				Destroy(link);
		}
	}
}
