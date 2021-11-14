using UnityEngine;

namespace Bug.InventorySystem
{
	public abstract class ItemAsset : ScriptableObject
	{
		public abstract Item ItemData { get; }

		public static implicit operator Item(ItemAsset asset) => asset.ItemData;
	}
}
