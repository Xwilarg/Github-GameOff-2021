using System.Collections.Generic;

namespace Bug.InventorySystem
{
	[System.Serializable]
	public class Recipe
	{
		public readonly List<ItemCount> requirements;
		public readonly List<ItemCount> results;


		public Recipe()
		{
			requirements = new List<ItemCount>();
			results = new List<ItemCount>();
		}

		public Recipe(IEnumerable<ItemCount> requirements, IEnumerable<ItemCount> results)
		{
			this.requirements = new List<ItemCount>(requirements);
			this.results = new List<ItemCount>(results);
		}
	}
}
