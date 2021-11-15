using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bug.InventorySystem
{
	public class Inventory : MonoBehaviour
	{
		private readonly List<InventoryEntry> _items = new();

		public event Action<InventoryOperation> OnInventoryUpdated;


		public void Add(Item item) => Add(item, 1);

		public void Add(Item item, int count)
		{
			InventoryEntry entry = GetOrCreateEntry(item);
			int cacheCount = entry.Count;
			entry.Count += count;

			InventoryOperation operation = new(InventoryOperationType.Addition, entry.item, cacheCount, count, entry.Count);
			OnInventoryUpdated?.Invoke(operation);
		}

		public void Remove(Item item, int count)
		{
			if (TryGetEntry(item, out InventoryEntry entry))
			{
				if (entry.Count <= count)
					RemoveEntry(entry);
				else
				{
					int cacheCount = entry.Count;
					entry.Count -= count;

					InventoryOperation operation = new(InventoryOperationType.Subtraction, entry.item, cacheCount, count, entry.Count);
					OnInventoryUpdated?.Invoke(operation);
				}
			}
		}

		public void RemoveAll(Item item)
		{
			if (TryGetEntry(item, out InventoryEntry entry))
				RemoveEntry(entry);
		}

		public void RemoveEntry(InventoryEntry entry)
		{
			int cacheCount = entry.Count;
			_items.Remove(entry);

			InventoryOperation operation = new(InventoryOperationType.Subtraction, entry.item, cacheCount, cacheCount, 0);
			OnInventoryUpdated?.Invoke(operation);
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

		public bool TryGetEntry(string itemName, out InventoryEntry entry)
		{
			entry = _items.FirstOrDefault(x => x.item.name == itemName);
			return entry != null;
		}

		public bool TryGetEntry(Item item, out InventoryEntry entry)
		{
			entry = _items.FirstOrDefault(x => x.item == item);
			return entry != null;
		}

		public bool HasItem(string itemName) => TryGetEntry(itemName, out InventoryEntry entry) && entry.Count > 0;

		public bool HasItem(Item item) => TryGetEntry(item, out InventoryEntry entry) && entry.Count > 0;

		public int GetItemCount(string itemName)
		{
			if (TryGetEntry(itemName, out InventoryEntry entry))
				return entry.Count;
			return 0;
		}

		public int GetItemCount(Item item)
		{
			if (TryGetEntry(item, out InventoryEntry entry))
				return entry.Count;
			return 0;
		}

		public bool HasRecipeRequirements(Recipe recipe) => recipe.requirements.All(r => GetItemCount(r.item) >= r.count);

		public void Clear() => _items.Clear();
	}
}
