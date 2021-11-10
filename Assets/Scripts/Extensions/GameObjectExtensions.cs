using UnityEngine;

namespace Bug
{
	public static class GameObjectExtensions
	{
		public static void SetLayerRecursively(this GameObject obj, int layer)
		{
			obj.layer = layer;

			foreach (Transform child in obj.transform)
			{
				child.gameObject.SetLayerRecursively(layer);
			}
		}
	}
}
