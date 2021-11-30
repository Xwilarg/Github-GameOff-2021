using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bug.Map
{
	public class RoomTracker : MonoBehaviour
	{
		public Room CurrentRoom => _roomStack.LastOrDefault();

		private List<Room> _roomStack = new List<Room>();


		public void OnEnterRoom(Room room)
		{
			if (!_roomStack.Contains(room))
				_roomStack.Add(room);
		}

		public void OnExitRoom(Room room)
		{
			if (_roomStack.Contains(room))
				_roomStack.Remove(room);
		}
	}
}
