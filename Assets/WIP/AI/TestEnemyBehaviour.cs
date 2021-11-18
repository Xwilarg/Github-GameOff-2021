using System;
using System.Collections;
using Bug.Player;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Bug
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class TestEnemyBehaviour : MonoBehaviour
	{
		[SerializeField] private State _currentState;
		[SerializeField] private TMP_Text _stateDebugDisplay;

		public State CurrentState { get => _currentState; set => SetState(value); }

		private NavMeshAgent _agent;

		private float _timeStateEntered;
		private float TimeInState => Time.unscaledTime - _timeStateEntered;

		private Vector3? _wanderDestination;


		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
		}

		private void Start()
		{
			_agent.isStopped = true;
		}

		private void Update()
		{
			PlayerBehaviour player = PlayerManager.GetPlayer();

			switch (_currentState)
			{
				case State.Idle:
				{
					if (TimeInState > 5f)
					{
						SetState(State.Seek);
						break;
					}
					break;
				}

				case State.Wander:
				{
					SetState(State.Idle);
					break;
				}

				case State.Seek:
				{
					if (player != null)
					{
						float distance = Vector3.Distance(player.transform.position, transform.position);
						if (distance < 2f)
						{
							_agent.isStopped = true;
							_agent.autoRepath = false;
							SetState(State.Combat);
							break;
						}
						if (_agent.hasPath && _agent.remainingDistance > 90f)
						{
							_agent.autoRepath = false;
							_agent.isStopped = true;
							_agent.ResetPath();
							SetState(State.Idle);
							break;
						}

						_agent.destination = player.transform.position;
						Debug.Log(_agent.destination);
						_agent.autoRepath = true;
						_agent.isStopped = false;
					}
					else
					{
						SetState(State.Idle);
						break;
					}
					break;
				}

				case State.Combat:
				{
					if (Vector3.Distance(player.transform.position, transform.position) > 4f)
					{
						SetState(State.Seek);
						break;
					}
					break;
				}
			}

			if (_stateDebugDisplay != null)
			{
				_stateDebugDisplay.text = _currentState.ToString().ToUpper();
			}
		}

		public void SetState(State state)
		{
			_currentState = state;
			_timeStateEntered = Time.unscaledTime;
		}

		public enum State
		{
			Idle,
			Wander,
			Seek,
			Combat
		}
	}
}
