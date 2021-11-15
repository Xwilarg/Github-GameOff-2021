using System;
using Bug.InventorySystem;
using UnityEngine;

namespace Bug.UI
{
	[RequireComponent(typeof(ItemRow))]
	public class ItemRowAutoUpdate : MonoBehaviour
	{
		[SerializeField] private ItemAsset _item;
		[SerializeField] private Inventory _inventory;

		public Inventory Inventory { get => _inventory; set => _inventory = value; }

		private bool _dirty;
		private ItemRow _row;


		private void Awake()
		{
			_row = GetComponent<ItemRow>();
			SetDirty();
		}

		private void OnEnable()
		{
			if (_item == null || _inventory == null)
				enabled = false;
		}

		private void Update()
		{
			_row.SubtitleLabel.text = _inventory.GetItemCount(_item.ItemData).ToString();
			if (_dirty)
			{
				_dirty = false;
				_row.TitleLabel.text = _item.ItemData.name;
				_row.Icon.sprite = _item.ItemData.icon;
			}
		}

		public void SetDirty()
		{
			_dirty = true;
			enabled = true;
		}
	}
}
