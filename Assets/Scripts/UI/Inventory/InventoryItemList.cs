using System;
using System.Collections.Generic;
using System.Linq;
using Bug.InventorySystem;
using UnityEngine;

namespace Bug.UI
{
	public class InventoryItemList : MonoBehaviour
	{
		[SerializeField] private List<ItemType> _filter;
		[SerializeField] private GameObject _rowPrefab;
		[SerializeField] private RectTransform _content;
		[SerializeField] private Inventory _inventory;

		private readonly Dictionary<Item, ItemRow> _rows = new();


		private void OnEnable()
		{
			if (_inventory != null)
			{
				_inventory.OnInventoryUpdated -= HandleOnInventoryUpdated;
				_inventory.OnInventoryUpdated += HandleOnInventoryUpdated;
			}
			Refresh();
		}

		private void OnDisable()
		{
			if (_inventory != null)
				_inventory.OnInventoryUpdated -= HandleOnInventoryUpdated;
		}

		public bool MatchFilter(Item item) => _filter == null || _filter.Count == 0 || _filter.Contains(item.type);

		public void Clear()
		{
			foreach (ItemRow row in _rows.Values)
				Destroy(row.gameObject);
			_rows.Clear();
		}

		public void SetInventory(Inventory inventory)
		{
			if (_inventory != null)
				_inventory.OnInventoryUpdated -= HandleOnInventoryUpdated;

			_inventory = inventory;

			if (_inventory != null)
				_inventory.OnInventoryUpdated += HandleOnInventoryUpdated;

			Refresh();
		}

		public void Refresh()
		{
			if (_inventory == null)
			{
				Clear();
				return;
			}

			List<Item> removeItems = _rows.Keys.Where(x => !_inventory.HasItem(x)).ToList();
			foreach (Item item in removeItems)
			{
				Destroy(_rows[item].gameObject);
				_rows.Remove(item);
			}

			foreach (InventoryEntry entry in _inventory.AllEntries().Where(x => MatchFilter(x.item)))
			{
				Item item = entry.item;
				if (!_rows.TryGetValue(item, out ItemRow row))
				{
					row = Instantiate(_rowPrefab, _content).GetComponent<ItemRow>();
					row.Icon.sprite = item.icon;
					row.TitleLabel.text = item.name;
					_rows.Add(item, row);
				}
				row.SubtitleLabel.text = entry.Count.ToString();
			}

			int index = 0;
			foreach (ItemRow row in _rows.OrderBy(x => x.Key.name).Select(x => x.Value))
				row.transform.SetSiblingIndex(index++);
		}

		private void HandleOnInventoryUpdated(InventoryOperation _)
		{
			Refresh();
		}
	}
}
