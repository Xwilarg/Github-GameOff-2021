using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bug
{

	[RequireComponent(typeof(NavMeshAgent))]
	public class PlayerFollower : MonoBehaviour
	{
		private NavMeshAgent _agent;

		private GameObject _player;


		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
		}

		private IEnumerator Start()
		{
			while (!FindPlayer(out _player))
				yield return null;
		}

		private bool FindPlayer(out GameObject player)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			return player != null;
		}

		public void Update()
		{
			if (_player != null)
			{
				_agent.destination = _player.transform.position;
			}
		}
	}
}
