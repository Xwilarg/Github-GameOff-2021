using System;
using System.Linq;
using Bug.Map;
using Unity.AI.Navigation;
using UnityEngine;

namespace Bug.Navigation
{
	public class NavMeshMapPostProcessor : MapPostProcessor
	{
		[SerializeField] private NavMeshSurface[] _surfaces;


		public override void Execute(MapManager manager)
		{
			Vector2 min = Vector2.zero;
			Vector2 max = Vector2.zero;

			foreach (Room room in manager.AllRooms)
			{
				Vector2 bottomLeft = room.Position;
				Vector2 topRight = room.Position + room.Size;
				if (bottomLeft.x < min.x) min.x = bottomLeft.x;
				if (bottomLeft.y < min.y) min.y = bottomLeft.y;
				if (topRight.x > max.x) max.x = topRight.x;
				if (topRight.y > max.y) max.y = topRight.y;
			}

			foreach (NavMeshSurface surface in _surfaces)
			{
				GrowAndBake(surface, min, max);
			}
		}

		private void GrowAndBake(NavMeshSurface surface, Vector2 min, Vector2 max)
		{
			surface.size = new Vector3(max.x - min.x, surface.size.y, max.y - min.y);
			surface.center = new Vector3(min.x + surface.size.x * 0.5f, surface.center.y, min.y + surface.size.z * 0.5f);
			surface.BuildNavMesh();
		}

		private void Reset()
		{
			_surfaces = GetComponentsInChildren<NavMeshSurface>();
		}
	}
}
