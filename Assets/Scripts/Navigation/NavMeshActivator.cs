using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Bug.Navigation
{
	public class NavMeshActivator : MonoBehaviour
	{
		[SerializeField] private float _distance = 30f;

		public bool SurfacesEnabled { get; private set; }

		private GameObject _player;
		private List<GameObject> _surfaces;


		private void Awake()
		{
			NavMeshSurface[] surfaces = GetComponentsInChildren<NavMeshSurface>();
			_surfaces = surfaces.Select(s => s.gameObject).Distinct().ToList();
		}

		private IEnumerator Start()
		{
			SetSurfacesEnabled(false);

			while (!FindPlayer(out _player))
				yield return null;
		}

		private bool FindPlayer(out GameObject player)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			return player != null;
		}

		private void Update()
		{
			bool shouldBeEnabled = false;

			if (_player != null)
			{
				float distance = Vector3.Distance(_player.transform.position, transform.position);
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
