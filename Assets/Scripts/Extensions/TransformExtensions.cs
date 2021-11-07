using UnityEngine;

namespace Bug
{
	public static class TransformExtensions
	{
		public static Transform FindChildRecursive(this Transform parent, string childName)
		{
			foreach (Transform child in parent)
			{
				if (child.name == childName)
				{
					return child;
				}
				else
				{
					Transform found = FindChildRecursive(child, childName);
					if (found != null)
					{
						return found;
					}
				}
			}
			return null;
		}
	}
}
