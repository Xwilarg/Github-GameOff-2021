using UnityEngine;

namespace Bug.InventorySystem
{
	[CreateAssetMenu(menuName = "ScriptableObject/Item", fileName = "New Item")]
	public class GenericItemAsset : ItemAsset
	{
		[SerializeField] private Item _item;

		public override Item ItemData => _item;
	}
}
