using System;
using Bug.Menu;
using Bug.Prop;
using Bug.SO;
using Bug.WeaponSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bug.Player
{
	public class PlayerController : MonoBehaviour, IPlayerController
	{
		public PlayerInfo _info;

		[Header("Movements")]
		[SerializeField]
		private Camera _fpsCamera;
		[SerializeField]
		private Transform _head;
		[SerializeField]
		private PlayerInput _playerInput;
		[SerializeField]
		[Range(0f, 1000000f)]
		private float _forceMultiplier = 1f;
		[Range(0, 10f)]
		[SerializeField] private float _horizontalLookMultiplier = 1f;
		[Range(0, 10f)]
		[SerializeField] private  float _verticalLookMultiplier = 1f;

		[Header("Skeleton")]
		[SerializeField] private Animator _armsAnimator;
		[SerializeField] private GameObject _skeletonRoot;
		[SerializeField] private string _leftHandAnchorName = "Anchor_Left";
		[SerializeField] private string _rightHandAnchorName = "Anchor_Right";

		[SerializeField]
		private GameObject _heldObject;

		private CarriedObject _carriedObject;
		public Camera Camera => _fpsCamera;
		public CharacterController Controller => _controller;
		public Animator Animator => _armsAnimator;

		public Transform LeftHandAnchor { get; private set; }
		public Transform RightHandAnchor { get; private set; }

		public GameObject HeldObject { get => _heldObject; set => _heldObject = value; }

		private CharacterController _controller;
		private float _headRotation;
		private Vector2 _groundMovement = Vector2.zero;

		private Interactible _eTarget;


		private void Awake()
		{
			LeftHandAnchor = _skeletonRoot.transform.FindChildRecursive(_leftHandAnchorName);
			RightHandAnchor = _skeletonRoot.transform.FindChildRecursive(_rightHandAnchorName);
		}

		private void Start()
		{
			_playerInput = GetComponent<PlayerInput>();
			_controller = GetComponent<CharacterController>();
			Cursor.lockState = CursorLockMode.Locked;

			if (_heldObject != null && _heldObject.TryGetComponent(out IHoldable holdable))
				holdable.HoldBegin(gameObject);
		}

		private void FixedUpdate()
		{
			if (IsGamePaused())
			{
				return; // We can't move if we are in the menu
			}

			if (PlayerManager.S != null && PlayerManager.S.PressE != null)
			{
				if (_carriedObject == null) // Since we are not carrying an object, we are able to pick one
				{
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
				}
				else // We are carrying an object, update position
				{
					PlayerManager.S.PressE.SetActive(false);
					_eTarget = null;

					_carriedObject.UpdatePosition(transform);
					_carriedObject.CanBePlaced();
				}
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

		public void PickObject(GameObject obj)
		{
			_carriedObject = new(transform, obj);
		}

		private bool IsGamePaused() => GameStateManager.Paused;

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

				transform.rotation *= Quaternion.AngleAxis(rot.x * _horizontalLookMultiplier, Vector3.up);

				_headRotation -= rot.y * _verticalLookMultiplier; // Vertical look is inverted by default, hence the -=

				_headRotation = Mathf.Clamp(_headRotation, -89, 89);
				_head.transform.localRotation = Quaternion.AngleAxis(_headRotation, Vector3.right);
			}
		}

		public void OnShoot(InputAction.CallbackContext value)
		{
			if (!IsGamePaused() && value.performed)
			{
				if (_heldObject != null && _carriedObject == null)
				{
					if (_heldObject.TryGetComponent(out IPrimaryActionHandler primaryActionHandler))
					{
						primaryActionHandler.PrimaryActionBegin();
					}
				}
			}
			else if (value.canceled && _heldObject != null && _carriedObject == null)
			{
				if (_heldObject.TryGetComponent(out IPrimaryActionHandler primaryActionHandler))
				{
					primaryActionHandler.PrimaryActionEnd();
				}
			}
		}

		public void OnReload(InputAction.CallbackContext value)
		{
			if (!IsGamePaused() && value.performed)
			{
				if (_heldObject != null && _carriedObject == null)
				{
					if (_heldObject.TryGetComponent(out IReloadHandler reloadHandler))
					{
						reloadHandler.ReloadRequested();
					}
				}
			}
		}

		public void OnMenu(InputAction.CallbackContext _)
		{
			GameStateManager.Paused = !GameStateManager.Paused;
			Cursor.lockState = GameStateManager.Paused ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = GameStateManager.Paused;
		}

		public void OnAction(InputAction.CallbackContext value)
		{
			if (value.phase == InputActionPhase.Performed)
			{
				if (_carriedObject == null) // We are not carrying an object so we interact with an object is available
				{
					_eTarget?.InvokeCallback(this);
				}
				else // Try to place the object we are carrying on the ground
				{
					if (_carriedObject.CanBePlaced())
					{
						_carriedObject.PlaceOnGround();
						_carriedObject = null;
					}
				}
			}
		}

		#endregion
	}
}
