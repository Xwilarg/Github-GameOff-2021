using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bug.InventorySystem
{
	public class Inventory : MonoBehaviour
	{
		private readonly List<InventoryEntry> _items = new();


		public void Add(Item item) => Add(item, 1);

		public void Add(Item item, int count)
		{
			InventoryEntry entry = GetOrCreateEntry(item);
			entry.Count += count;
		}

		public void Remove(Item item, int count)
		{
			if (TryGetEntry(item, out InventoryEntry entry))
				entry.Count = Mathf.Max(entry.Count - count, 0);
		}

		public void RemoveAll(Item item)
		{
			if (TryGetEntry(item, out InventoryEntry entry))
				entry.Count = 0;
		}

		public InventoryEntry GetOrCreateEntry(Item item)
		{
			if (!TryGetEntry(item, out InventoryEntry entry))
			{
				entry = new InventoryEntry(item);
				_items.Add(entry);
			}
			return entry;
		}

		public IEnumerable<InventoryEntry> AllItems() => _items;

		public InventoryEntry GetEntry(string itemName) => _items.FirstOrDefault(x => x.item.name == itemName);

		public InventoryEntry GetEntry(Item item) => _items.FirstOrDefault(x => x.item == item);

		public bool TryGetEntry(Item item, out InventoryEntry entry)
		{
			entry = GetEntry(item);
			return entry != null;
		}

		public bool HasItem(string itemName) => _items.Any(x => x.item.name == itemName && x.Count > 0);

		public bool HasItem(Item item) => _items.Any(x => x.item == item && x.Count > 0);

		public int GetItemCount(Item item)
		{
			InventoryEntry entry = GetEntry(item);
			if (entry != null)
				return entry.Count;
			return 0;
		}

		public bool HasRecipeRequirements(Recipe recipe) => recipe.requirements.All(requirement => GetItemCount(requirement.item) >= requirement.count);

		public void Clear() => _items.Clear();
	}
}
