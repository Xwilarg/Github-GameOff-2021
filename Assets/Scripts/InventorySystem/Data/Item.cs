using UnityEngine;

namespace Bug.InventorySystem
{
	[System.Serializable]
	public class Item
	{
		public ItemType type = ItemType.Resource;
		public string name;
		[LargeSprite]
		public Sprite icon;
		public GameObject prefab;
	}
}
