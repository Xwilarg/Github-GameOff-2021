using Bug.Menu;
using Bug.Prop;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bug.Player
{
	public class PlayerController : MonoBehaviour
	{
		[Header("Movements")]
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

		[Header("Shooting")]
		[SerializeField]
		private GameObject _bulletPrefab;
		[SerializeField]
		private Transform _gunEnd;
		[SerializeField]
		private int _gunForce;

		private CharacterController _controller;
		private Vector2 _groundMovement = Vector2.zero;
		private Rigidbody _rb;

		private Interactible _eTarget;
		
		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
			_playerInput = GetComponent<PlayerInput>();
			_controller = GetComponent<CharacterController>();
			Cursor.lockState = CursorLockMode.Locked;
		}
		
		private void FixedUpdate()
		{
			if (IsGamePaused())
            {
				return; // We can't move if we are in the menu
            }

			if (Physics.Raycast(new Ray(_fpsCamera.transform.position, _fpsCamera.transform.forward), out RaycastHit hit, 1f))
            {
				var interac = hit.collider.GetComponent<Interactible>();
				if (interac != null)
                {
					PlayerManager.S.PressE.SetActive(true);
					_eTarget = interac;

				}
				else
				{
					PlayerManager.S.PressE.SetActive(false);
					_eTarget = null;
				}
			}
			else
			{
				PlayerManager.S.PressE.SetActive(false);
				_eTarget = null;
			}

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

		private bool IsGamePaused()
			=> PlayerManager.S.PauseMenu.IsActive();

        #region Input callbacks

        public void OnMovement(InputAction.CallbackContext value)
		{
			_groundMovement = value.ReadValue<Vector2>().normalized;
		}
		
		public void OnLook(InputAction.CallbackContext value)
		{
			if (!IsGamePaused())
			{
				var rot = value.ReadValue<Vector2>();
				transform.rotation *= Quaternion.Euler(0f, rot.x * _horizontalLookMultiplier, 0f);
				_fpsCamera.transform.rotation *= Quaternion.Euler(rot.y * _verticalLookMultiplier, 0f, 0f);
			}
		}

		public void OnShoot(InputAction.CallbackContext value)
        {
			if (value.phase == InputActionPhase.Performed && !IsGamePaused())
            {
				var go = Instantiate(_bulletPrefab, _gunEnd.position, Quaternion.identity);
				go.GetComponent<Rigidbody>().AddForce(_gunEnd.forward * _gunForce, ForceMode.Impulse);
				Destroy(go, 2f);
            }
        }

		public void OnMenu(InputAction.CallbackContext value)
        {
			PlayerManager.S.PauseMenu.Toggle();
			Cursor.lockState = PlayerManager.S.PauseMenu.IsActive() ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = PlayerManager.S.PauseMenu.IsActive();
        }

		public void OnAction(InputAction.CallbackContext value)
        {
			_eTarget?.InvokeCallback(this);
		}

        #endregion
    }
}