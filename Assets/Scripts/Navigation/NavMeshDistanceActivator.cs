using System.Collections.Generic;
using System.Linq;
using Bug.Player;
using Unity.AI.Navigation;
using UnityEngine;

namespace Bug.Navigation
{
	public class NavMeshDistanceActivator : MonoBehaviour
	{
		[SerializeField] private List<GameObject> _surfaces;
		[SerializeField] private float _distance = 30f;

		public bool SurfacesEnabled { get; private set; }


		private void Awake()
		{
			NavMeshSurface[] surfaces = GetComponentsInChildren<NavMeshSurface>();
			_surfaces = surfaces.Select(s => s.gameObject).Distinct().ToList();
		}

		private void Start()
		{
			SetSurfacesEnabled(false);
		}

		private void Update()
		{
			bool shouldBeEnabled = false;

			PlayerBehaviour player = PlayerManager.GetPlayer();
			if (player != null)
			{
				float distance = Vector3.Distance(player.transform.position, transform.position);
				shouldBeEnabled = distance <= _distance;
			}

			if (shouldBeEnabled != SurfacesEnabled)
			{
				SetSurfacesEnabled(shouldBeEnabled);
			}
		}

		public void SetSurfacesEnabled(bool enabled)
		{
			SurfacesEnabled = enabled;

			foreach (GameObject surface in _surfaces)
			{
				surface.SetActive(enabled);
			}
		}
	}
}
