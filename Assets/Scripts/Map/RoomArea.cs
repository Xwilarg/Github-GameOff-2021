using System;
using UnityEngine;

namespace Bug.Map
{
	[RequireComponent(typeof(BoxCollider))]
	public class RoomArea : MonoBehaviour
	{
		private Room _room;


		private void Start()
		{
			_room = GetComponentInParent<Room>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody != null && other.attachedRigidbody.TryGetComponent(out RoomTracker roomTracker))
			{
				roomTracker.OnEnterRoom(_room);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.attachedRigidbody != null && other.attachedRigidbody.TryGetComponent(out RoomTracker roomTracker))
			{
				roomTracker.OnExitRoom(_room);
			}
		}

		private void Reset()
		{
			GetComponent<BoxCollider>().isTrigger = true;
			gameObject.layer = LayerMask.NameToLayer("Areas");
		}
	}
}
