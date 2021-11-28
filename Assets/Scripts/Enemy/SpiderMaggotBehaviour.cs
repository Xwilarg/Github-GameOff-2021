using System;
using System.Collections;
using System.Collections.Generic;
using Bug.Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Bug.Enemy
{
	public class SpiderMaggotBehaviour : MonoBehaviour, IDamageHandler
	{
		[SerializeField] private State _currentState = State.Idle;
		[SerializeField] private HealthPool _healthPool;

		[SerializeField] private float _aggroDistance = 30f;
		[SerializeField] private float _blowUpDistance = 1f;
		[SerializeField] private float _blowUpDelay = 1f;
		[SerializeField] private float _blowUpRadius = 1f;
		[SerializeField] private float _blowUpDamage = 40f;
		[SerializeField] private LayerMask _explosionLayerMask = -1;
		[SerializeField] private AnimationCurve _blowUpDamageOverDistance;
		[SerializeField] private GameObject _explosionPrefab;

		public State CurrentState { get => _currentState; set => SetState(value); }
		public HealthPool HealthPool => _healthPool;

		private float _timeStateEntered;
		private float TimeInState => Time.unscaledTime - _timeStateEntered;

		private NavMeshAgent _agent;
		private Animator _animator;

		private readonly int _walkingAnimProp = Animator.StringToHash("Walking");

		private float _speed;
		private float _angularSpeed;
		private float _movementSpeed;
		private bool _blowingUp;

		private Coroutine _setSpeedCoroutine;


		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
			_animator = GetComponent<Animator>();
		}

		private void Start()
		{
			_speed = _agent.speed;
			_angularSpeed = _agent.angularSpeed;

			if (_currentState == State.Idle)
			{
				StopAgent();
				SetMovementSpeedFactor(0f);
			}
		}

		private void OnEnable()
		{
			_healthPool.OnDepleted += HandleOnHealthPoolDepleted;
		}

		private void OnDisable()
		{
			_healthPool.OnDepleted -= HandleOnHealthPoolDepleted;
		}

		private void Update()
		{
			PlayerBehaviour player = PlayerManager.GetPlayer();

			switch (_currentState)
			{
				case State.Idle:
				{
					SetDestination(player.transform.position);
					_animator.SetBool(_walkingAnimProp, false);

					if (_agent.hasPath && _agent.remainingDistance <= _aggroDistance)
					{
						SetState(State.Chasing);
						StartAgent();
						break;
					}

					break;
				}

				case State.Chasing:
				{
					SetDestination(player.transform.position);
					_animator.SetBool(_walkingAnimProp, true);

					if (_agent.hasPath)
					{
						if (_agent.remainingDistance > _aggroDistance)
						{
							StopAgent();
							SetState(State.Idle);
						}
						else if (_agent.remainingDistance <= _blowUpDistance)
						{
							SetState(State.BlowUp);
						}
					}

					break;
				}

				case State.BlowUp:
				{
					if (_blowingUp) break;

					StopAgent();
					_animator.SetBool(_walkingAnimProp, false);
					SetMovementSpeedAnimated(0, 0);

					StartCoroutine(BlowUpCoroutine());

					break;
				}

			}
		}

		private IEnumerator BlowUpCoroutine()
		{
			_blowingUp = true;
			yield return new WaitForSeconds(_blowUpDelay);
			yield return StartCoroutine(BlowUpNowCoroutine());
		}

		public void BlowUpNow()
		{
			StartCoroutine(BlowUpNowCoroutine());
		}

		private IEnumerator BlowUpNowCoroutine()
		{
			_blowingUp = true;

			Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

			RaycastHit[] hits = Physics.SphereCastAll(transform.position, _blowUpRadius, Vector3.forward, 100f, _explosionLayerMask);

			foreach (RaycastHit hit in hits)
			{
				GameObject target = hit.rigidbody != null ? hit.rigidbody.gameObject : hit.collider.gameObject;

				if (target == gameObject) continue;

				if (target.TryGetComponent(out SpiderMaggotBehaviour otherMaggot))
				{
					otherMaggot.SetState(State.BlowUp);
				}
				else if (target.TryGetComponent(out IDamageHandler damageHandler))
				{
					float distance = Vector3.Distance(target.transform.position, transform.position);
					float damageFactor = _blowUpDamageOverDistance.Evaluate(distance / _blowUpRadius);
					damageHandler.TakeDamage(_blowUpDamage * damageFactor);
				}
			}

			Destroy(gameObject);
			yield break;
		}

		public void Anim_SetMovementFactor(float arg0)
		{
			SetMovementSpeedAnimated(arg0, 0.5f);
		}

		private void StopMovementSpeedAnimation()
		{
			if (_setSpeedCoroutine != null)
				StopCoroutine(_setSpeedCoroutine);
		}

		private void SetDestination(Vector3 destination)
		{
			_agent.SetDestination(destination);
		}

		private void StartAgent()
		{
			_agent.isStopped = false;
		}

		private void StopAgent()
		{
			_agent.isStopped = true;
		}

		private void SetMovementSpeedFactor(float movementSpeed)
		{
			_movementSpeed = movementSpeed;
			_agent.speed = movementSpeed * _speed;
			_agent.angularSpeed = movementSpeed * _angularSpeed;
		}

		private void SetMovementSpeedAnimated(float movementSpeed, float duration)
		{
			StopMovementSpeedAnimation();
			_setSpeedCoroutine = StartCoroutine(SetMovementSpeedCoroutine(movementSpeed, duration));
		}

		private IEnumerator SetMovementSpeedCoroutine(float movementSpeed, float duration)
		{
			float time = Time.deltaTime;
			float startSpeed = _movementSpeed;

			while (time < duration)
			{
				float ratio = time / duration;
				float speed = Mathf.Lerp(startSpeed, movementSpeed, ratio);
				SetMovementSpeedFactor(speed);
				yield return null;
				time += Time.deltaTime;
			}

			SetMovementSpeedFactor(movementSpeed);
			_setSpeedCoroutine = null;
		}

		public void SetState(State state)
		{
			_currentState = state;
			_timeStateEntered = Time.unscaledTime;
		}

		private void OnDrawGizmosSelected()
		{
#if UNITY_EDITOR
			using (new Handles.DrawingScope(new Color(1, 0.7052167f, 0f)))
				Handles.DrawWireDisc(transform.position, Vector3.up, _blowUpDistance);
			using (new Handles.DrawingScope(Color.yellow))
				Handles.DrawWireDisc(transform.position, Vector3.up, _aggroDistance);
#endif // UNITY_EDITOR
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, _blowUpRadius);
		}

		private void HandleOnHealthPoolDepleted()
		{
			SetState(State.BlowUp);
		}

		public void TakeDamage(float delta)
		{
			_healthPool.Modify(-delta);
		}

		public void Heal(float delta) { }

		public enum State
		{
			Idle,
			Chasing,
			BlowUp
		}
	}
}
