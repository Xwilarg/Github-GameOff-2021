namespace Bug.WeaponSystem
{
	[System.Serializable]
	public struct RecoilData
	{
		public float x;
		public float y;
		public float z;


		public RecoilData(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static RecoilData operator *(RecoilData recoilData, float factor)
		{
			return new RecoilData {
				x = recoilData.x * factor,
				y = recoilData.y * factor,
				z = recoilData.z * factor
			};
		}
	}
}
