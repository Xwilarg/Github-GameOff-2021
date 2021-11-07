using UnityEngine;

namespace Bug.Map
{
	public abstract class MapPostProcessor : MonoBehaviour
	{
		public abstract void Execute(MapManager manager);
	}
}
