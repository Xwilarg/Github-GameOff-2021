namespace Bug.WeaponSystem
{
	public class AmmoData
	{
		public int Capacity { get; set; }

		public int Count { get; set; }


		public AmmoData(int capacity, int count)
		{
			Capacity = capacity;
			Count = count;
		}
	}
}
