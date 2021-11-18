using UnityEngine;

namespace Bug.InventorySystem
{
	public class ItemBehaviour : MonoBehaviour
	{
		[SerializeField] private ItemAsset m_Item;
		[SerializeField] private int m_Count = 1;

		public ItemAsset Item { get => m_Item; set => m_Item = value; }
		public int Count { get => m_Count; set => m_Count = value; }
	}
}
