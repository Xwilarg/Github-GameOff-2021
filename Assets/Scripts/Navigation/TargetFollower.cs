using System;
using UnityEngine;
using UnityEngine.AI;

namespace Bug.Navigation
{
	/// <summary>
	/// Updates a NavMeshAgent target position.
	/// </summary>
	[RequireComponent(typeof(NavMeshAgent))]
	public class TargetFollower : MonoBehaviour
	{
		[SerializeField] private Transform _target;

		private NavMeshAgent _agent;


		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
		}

		private void Update()
		{
			_agent.destination = _target.position;
		}
	}
}
