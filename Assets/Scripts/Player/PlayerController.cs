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
		
		private bool _onGround = false;
		private Vector2 _groundMovement = Vector2.zero;
		private Rigidbody _rb;
		
		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
			_playerInput = GetComponent<PlayerInput>();
			Cursor.lockState = CursorLockMode.Locked;
		}
		
		private void FixedUpdate()
		{
			//#TODO Check that player is on the ground. Use raycasting.
			/* Ground Movement
			* restrict movement to the xz plane.
			*/

			var mov = (transform.forward * _groundMovement.y + transform.right * _groundMovement.x) * _forceMultiplier;
			_rb.velocity = new Vector3(mov.x, _rb.velocity.y, mov.z);

			/*_charMov.x = _groundMovement.x;
			_charMov.z = _groundMovement.y;
			// Vector3 groundDirection = _fpsCamera.transform.forward;
			Vector3 groundDirection = _fpsCamera.transform.rotation.eulerAngles;
			groundDirection.x = groundDirection.z = 0f; // Restrict the angle to the Y axis
			groundDirection.Normalize();
			Vector3 force = Quaternion.Euler(groundDirection) * _charMov * _forceMultiplier * Time.fixedDeltaTime;
			_rb.AddForce(force, ForceMode.Acceleration);*/

			// Looking around
			// apply vertical rotation to the head
			// apply horizontal rotation to the body

			// the mouse delta stuff is a mess... gotta' undo/recalculate their accumulations on updates, to be able to use it here [in fixed updates]


			/* _fpsCamera.transform.rotation = Quaternion.Euler(
				(_fpsCamera.transform.rotation.eulerAngles 
				+ (new Vector3(
						_cursorMovement.y * _horizontalLookMultiplier * Time.fixedDeltaTime,
						_cursorMovement.x * _verticalLookMultiplier * Time.fixedDeltaTime,
						0)
					)).normalized); */

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
		
		//#TODO ask about jumping. Implement it, if needed.
	}
}