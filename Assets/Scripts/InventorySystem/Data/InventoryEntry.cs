namespace Bug.InventorySystem
{
	public class InventoryEntry
	{
		public readonly Item item;

		public int Count { get; set; }


		public InventoryEntry(Item item)
		{
			this.item = item;
		}
	}
}
