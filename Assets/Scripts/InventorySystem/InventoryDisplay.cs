using System;
using UnityEngine;

namespace Bug.InventorySystem
{
	public class InventoryDisplay : MonoBehaviour
	{
		[SerializeField] private GameObject _display;

		public bool Showing { get; private set; }


		private void Start()
		{
			Hide();
		}

		public void Show()
		{
			Showing = true;
			_display.SetActive(true);
		}

		public void Hide()
		{
			Showing = false;
			_display.SetActive(false);
		}

		public void Toggle()
		{
			if (Showing)
				Hide();
			else
				Show();
		}
	}
}
