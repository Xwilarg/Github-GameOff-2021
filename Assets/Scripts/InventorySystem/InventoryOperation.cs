namespace Bug.InventorySystem
{
	public struct InventoryOperation
	{
		public InventoryOperationType type;

		public Item item;

		public int originalCount;
		public int operationCount;
		public int updatedCount;


		public InventoryOperation(InventoryOperationType type, Item item) : this(type, item, 0, 0, 0) { }

		public InventoryOperation(InventoryOperationType type, Item item, int originalCount, int operationCount, int updatedCount)
		{
			this.type = type;
			this.item = item;
			this.originalCount = originalCount;
			this.operationCount = operationCount;
			this.updatedCount = updatedCount;
		}
	}
}
