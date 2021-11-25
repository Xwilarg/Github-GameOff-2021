using UnityEngine;

namespace Bug
{
	public static class RandomUtility
	{
		public static float MinMax(Vector2 minMax)
		{
			return Random.Range(minMax.x, minMax.y);
		}
	}
}
