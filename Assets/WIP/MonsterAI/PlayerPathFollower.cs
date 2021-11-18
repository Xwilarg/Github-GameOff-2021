using System;
using System.Collections;
using System.Collections.Generic;
using Bug.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Bug
{

	[RequireComponent(typeof(NavMeshAgent))]
	public class PlayerPathFollower : MonoBehaviour
	{
		private NavMeshAgent _agent;


		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
		}

		public void Update()
		{
			PlayerBehaviour player = PlayerManager.GetPlayer();
			if (player != null)
			{
				_agent.destination = player.transform.position;
			}
		}
	}
}
