using System;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMesh : MonoBehaviour
{
	public NavMeshAgent[] agents;

	public Transform target;


	private void Update()
	{
		foreach (NavMeshAgent agent in agents)
		{
			agent.destination = target.position;
		}
	}
}
