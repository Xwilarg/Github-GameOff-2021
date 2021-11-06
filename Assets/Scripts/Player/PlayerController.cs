using UnityEngine;
using UnityEngine.InputSystem;

namespace Bug.Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField]
		private Camera _fpsCamera;
		[SerializeField]
		private PlayerInput _playerInput;
		[SerializeField]
		[Range(0f, 1000000f)]
		private float _forceMultiplier = 1f;
		[SerializeField]
		[Range(-1f, 1f)]
		private float _horizontalLookMultiplier = 1f, _verticalLookMultiplier = 1f;

		private CharacterController _controller;
		private Vector2 _groundMovement = Vector2.zero;
		private Rigidbody _rb;
		
		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
			_playerInput = GetComponent<PlayerInput>();
			_controller = GetComponent<CharacterController>();
			Cursor.lockState = CursorLockMode.Locked;
		}
		
		private void FixedUpdate()
		{
			Vector3 desiredMove = transform.forward * _groundMovement.y + transform.right * _groundMovement.x;

			// Get a normal for the surface that is being touched to move along it
			Physics.SphereCast(transform.position, _controller.radius, Vector3.down, out RaycastHit hitInfo,
							   _controller.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
			desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

			Vector3 moveDir = Vector3.zero;
			moveDir.x = desiredMove.x * _forceMultiplier;
			moveDir.z = desiredMove.z * _forceMultiplier;


			if (_controller.isGrounded)
			{
				moveDir.y = -.1f;
			}
			else
			{
				moveDir += Physics.gravity;
			}
			_controller.Move(moveDir);
		}

		public void OnMovement(InputAction.CallbackContext value)
		{
			// Vector2 inputMovement = value.ReadValue<Vector2>().normalized;
			_groundMovement = value.ReadValue<Vector2>().normalized;
		}
		
		public void OnLook(InputAction.CallbackContext value)
		{
			var rot = value.ReadValue<Vector2>();
			transform.rotation *= Quaternion.Euler(0f, rot.x * _horizontalLookMultiplier, 0f);
			_fpsCamera.transform.rotation *= Quaternion.Euler(rot.y * _verticalLookMultiplier, 0f, 0f);
		}
	}
}