using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bug.InventorySystem
{
	[CreateAssetMenu(menuName = "ScriptableObject/Recipe", fileName = "New Recipe")]
	public class RecipeAsset : ScriptableObject
	{
		[SerializeField] private List<SerializedEntry> _requirements = new();
		[SerializeField] private List<SerializedEntry> _results = new();

		private Recipe _recipeData;
		public Recipe RecipeData => _recipeData ??= new Recipe(_requirements.Select(x => x.ToRuntime()), _results.Select(x => x.ToRuntime()));

		public static implicit operator Recipe(RecipeAsset asset) => asset.RecipeData;


		[System.Serializable]
		private class SerializedEntry
		{
			public ItemAsset item;
			public int count = 1;

			public ItemCount ToRuntime() => new(item.ItemData, count);
		}
	}
}
