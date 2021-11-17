namespace Bug.InventorySystem
{
	public class ItemCount
	{
		public readonly Item item;
		public readonly int count;


		public ItemCount(Item item) : this(item, 1) { }

		public ItemCount(Item item, int count)
		{
			this.item = item;
			this.count = count;
		}
	}
}
