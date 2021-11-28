using System;
using System.Collections;
using Bug.Player;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Bug.Enemy
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class MantisBehaviour : MonoBehaviour
	{
		[SerializeField] private State _currentState = State.Wander;

		[SerializeField] private float _aggroRange = 10f;
		[SerializeField] private LayerMask _sightBlockLayerMask = -1;

		[Header("Rig")]
		[SerializeField] private Transform _rigRoot;
		[SerializeField] private string _headBoneName = "Head";

		[Header("Jump")]
		[SerializeField] private Vector2 _jumpRange = new Vector2(6f, 4f);
		[SerializeField] private float _jumpWarningDuration = 0.7f;
		[SerializeField] private float _jumpDuration = 1f;
		[SerializeField] private float _jumpDistFromPlayer = 0.8f;
		[SerializeField] private Vector2 _jumpCooldownMinMax = new Vector2(5f, 10f);
		[SerializeField] private AnimationCurve _jumpTranslationCurve = AnimationCurve.Constant(0, 1, 1);
		[SerializeField] private float _jumpHeight = 2f;
		[SerializeField] private AnimationCurve _jumpHeightCurve = AnimationCurve.Constant(0, 1, 1);

		[Header("Melee")]
		[SerializeField] private float _meleeDamage = 20f;
		[SerializeField] private float _meleeDistance = 1f;
		[SerializeField] private Vector2 _attackCooldownMinMax = new Vector2(0.8f, 1.4f);
		[SerializeField] private BoxCollider _meleeBoxCollider;
		[SerializeField] private LayerMask _meleeLayerMask = -1;

		[Header("Movement")]
		[Tooltip("x: Minimum velocity, y: Maximum velocity, z: Minimum speed, w: Maximum speed.")]
		[SerializeField] private Vector4 _moveAnimationSpeed;

		public State CurrentState { get => _currentState; set => SetState(value); }

		private float _timeStateEntered;
		private float TimeInState => Time.unscaledTime - _timeStateEntered;

		private NavMeshAgent _agent;
		private Animator _animator;
		private Transform _headTransform;

		private readonly int _moveSpeedAnimProperty = Animator.StringToHash("WalkSpeedFactor");
		private readonly int _attackTriggerProperty = Animator.StringToHash("Attack");
		private readonly int _jumpTriggerProperty = Animator.StringToHash("Jump");
		private readonly int _pinchersOpenedProperty = Animator.StringToHash("PinchersOpened");

		private bool _jumping;
		private float _nextJumpTime;
		private float _nextAttackTime;


		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
			_animator = GetComponent<Animator>();

			_meleeBoxCollider.enabled = false;
			_headTransform = _rigRoot.FindChildRecursive(_headBoneName);
		}

		private void Update()
		{
			PlayerBehaviour player = PlayerManager.GetPlayer();

			switch (_currentState)
			{
				case State.Wander:

					if (IsTargetInAggroRange(player.transform))
					{
						SetState(State.Aggressive);
						break;
					}

					_animator.SetBool(_pinchersOpenedProperty, false);
					UpdateMovementAnimSpeedFromAgent();

					break;

				case State.Aggressive:

					if (_jumping) break;

					if (!IsTargetInAggroRange(player.transform))
					{
						StopAgent();
						SetState(State.Wander);
					}
					else if (IsTargetInMeleeRange(player.transform))
						SetState(State.Melee);
					else if (IsTargetInJumpRange(player.transform) && CanJump(player.transform) && PlayerLooking(player))
					{
						StartCoroutine(JumpCoroutine(player.transform, false));
					}
					else
					{
						SetAgentDestination(player.transform.position);
						UpdateMovementAnimSpeedFromAgent();
					}

					break;

				case State.Melee:

					if (!IsTargetInMeleeRange(player.transform))
					{
						SetState(State.Aggressive);
						break;
					}

					_animator.SetBool(_pinchersOpenedProperty, true);
					UpdateMovementAnimSpeedFromAgent();

					if (CanAttack())
					{
						StartCoroutine(AttackCoroutine());
					}

					break;
			}
		}

		private IEnumerator AttackCoroutine()
		{
			_nextAttackTime = Time.time + RandomUtility.MinMax(_attackCooldownMinMax);
			_animator.SetTrigger(_attackTriggerProperty);

			Quaternion orientation = _meleeBoxCollider.transform.rotation;
			Vector3 center = transform.position + (orientation * _meleeBoxCollider.center);
			Vector3 size = _meleeBoxCollider.size * 0.5f;

			RaycastHit[] hits = Physics.BoxCastAll(center, size, Vector3.forward, orientation, 0f, _meleeLayerMask);

			foreach (RaycastHit hit in hits)
			{
				GameObject target = hit.rigidbody != null ? hit.rigidbody.gameObject : hit.collider.gameObject;
				if (target.TryGetComponent(out IDamageHandler damageHandler))
					damageHandler.TakeDamage(_meleeDamage);
			}

			yield break;
		}

		private IEnumerator JumpCoroutine(Transform target, bool preCalculate = false)
		{
			Vector3 targetPosition = target.position;
			if (preCalculate)
				targetPosition = CalculateJumpTargetPosition(target);

			_jumping = true;
			StopAgent(false);

			_animator.SetBool(_pinchersOpenedProperty, true);

			float warningTime = Time.deltaTime;
			while (warningTime < _jumpWarningDuration)
			{
				UpdateMovementAnimSpeedFromAgent();
				yield return null;
				warningTime += Time.deltaTime;
			}

			_animator.SetTrigger(_jumpTriggerProperty);

			if (!preCalculate)
				targetPosition = CalculateJumpTargetPosition(target);

			float time = Time.deltaTime;
			Vector3 startPosition = transform.position;

			while (time < _jumpDuration)
			{
				float ratio = time / _jumpDuration;
				ratio = _jumpTranslationCurve.Evaluate(ratio);
				Vector3 position = Vector3.Lerp(startPosition, targetPosition, ratio);
				position.y += _jumpHeightCurve.Evaluate(ratio) * _jumpHeight;
				UpdateMovementAnimSpeed(Vector3.Distance(transform.position, position));
				transform.position = position;

				yield return null;
				time += Time.deltaTime;
			}

			StartCoroutine(AttackCoroutine());

			_jumping = false;
			_nextJumpTime = Time.time + RandomUtility.MinMax(_jumpCooldownMinMax);
		}

		private void UpdateMovementAnimSpeedFromAgent() => UpdateMovementAnimSpeed(_agent.velocity.magnitude);

		private void UpdateMovementAnimSpeed(float velocity)
		{
			float t = Mathf.InverseLerp(_moveAnimationSpeed.x, _moveAnimationSpeed.y, velocity);
			float speed = Mathf.Lerp(_moveAnimationSpeed.z, _moveAnimationSpeed.w, t);
			_animator.SetFloat(_moveSpeedAnimProperty, speed);
		}

		private void SetAgentDestination(Vector3 destination)
		{
			_agent.SetDestination(destination);
			_agent.autoRepath = true;
			_agent.isStopped = false;
			_agent.updatePosition = true;
			_agent.updateRotation = true;
		}

		private void StopAgent(bool immediately = false)
		{
			_agent.autoRepath = false;
			_agent.isStopped = true;

			if (immediately)
			{
				_agent.updatePosition = false;
				_agent.velocity = Vector3.zero;
			}
		}

		private Vector3 CalculateJumpTargetPosition(Transform target)
		{
			Vector3 delta = target.transform.position - transform.position;
			return target.transform.position - delta.normalized * _jumpDistFromPlayer;
		}

		private bool TargetInSight(Transform target) => TargetInSight(target, Vector3.zero);

		private bool TargetInSight(Transform target, Vector3 offset)
		{
			Vector3 start = _headTransform.position;
			Vector3 end = target.position + offset;

			Ray ray = new Ray(start, end - start);

			bool hasHit = Physics.SphereCast(ray, 0.3f, 100f, _sightBlockLayerMask);
			Debug.DrawLine(start, end, hasHit ? Color.red : Color.green);

			return !hasHit;
		}

		private bool CanAttack()
		{
			return Time.time >= _nextAttackTime;
		}

		private bool CanJump(Transform target)
		{
			return !_jumping && Time.time >= _nextJumpTime && TargetInSight(target, Vector3.up);
		}

		private bool PlayerLooking(PlayerBehaviour player)
		{
			Vector3 delta = transform.position - player.Controller.Camera.transform.position;
			return Vector3.Dot(player.Controller.Camera.transform.forward, delta.normalized) > 0.5f;
		}

		private bool IsTargetInMeleeRange(Transform target)
		{
			return Vector3.Distance(transform.position, target.position) <= _meleeDistance;
		}

		private bool IsTargetInJumpRange(Transform target)
		{
			float distance = Vector3.Distance(transform.position, target.position);
			return  distance <= _jumpRange.x && distance >= _jumpRange.y;
		}

		private bool IsTargetInAggroRange(Transform target)
		{
			return Vector3.Distance(transform.position, target.position) <= _aggroRange;
		}

		public void SetState(State state)
		{
			_currentState = state;
			_timeStateEntered = Time.unscaledTime;
		}

		private void OnDrawGizmosSelected()
		{
#if UNITY_EDITOR
			using (new Handles.DrawingScope(Color.red))
				Handles.DrawWireDisc(transform.position, Vector3.up, _meleeDistance);
			using (new Handles.DrawingScope(Color.yellow))
				Handles.DrawWireDisc(transform.position, Vector3.up, _aggroRange);
			using (new Handles.DrawingScope(Color.green))
			{
				Handles.DrawWireDisc(transform.position, Vector3.up, _jumpRange.x);
				Handles.DrawWireDisc(transform.position, Vector3.up, _jumpRange.y);
			}
#endif // UNITY_EDITOR
		}

		public enum State
		{
			Wander,
			Aggressive,
			Melee
		}
	}
}
