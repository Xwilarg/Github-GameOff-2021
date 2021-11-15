using System;
using Bug.InventorySystem;
using UnityEngine;

namespace Bug
{
	public class TestInventory : MonoBehaviour
	{
		[SerializeField] private Inventory m_Inventory;


		private void Awake()
		{
			m_Inventory.OnInventoryUpdated += HandleOnInventoryUpdated;
		}

		private void HandleOnInventoryUpdated(InventoryOperation operation)
		{
			Debug.Log($"{(operation.type == InventoryOperationType.Addition ? "Added" : "Removed")} {operation.operationCount} {operation.item.name} {(operation.type == InventoryOperationType.Addition ? "to" : "from")} inventory.");
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
				RaycastAndAddOrRemove(true);
			else if (Input.GetMouseButtonDown(1))
				RaycastAndAddOrRemove(false);
		}


		private void RaycastAndAddOrRemove(bool add)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit, 1000, -1) && hit.rigidbody.TryGetComponent(out ItemBehaviour itemBehaviour))
			{
				int count = itemBehaviour.Count;

				if (Input.GetKey(KeyCode.LeftShift))
					count *= 2;
				else if (Input.GetKey(KeyCode.LeftControl))
					count = 1;

				if (add)
					m_Inventory.Add(itemBehaviour.Item, count);
				else
					m_Inventory.Remove(itemBehaviour.Item, count);
			}
		}
	}
}
