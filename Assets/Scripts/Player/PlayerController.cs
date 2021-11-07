using System;
using Bug.Menu;
using Bug.Prop;
using Bug.SO;
using Bug.Weapon;
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
		[SerializeField]
		[Range(-1f, 1f)]
		private float _horizontalLookMultiplier = 1f, _verticalLookMultiplier = 1f;

		[Header("Skeleton")]
		[SerializeField] private GameObject _skeletonRoot;
		[SerializeField] private string _leftHandAnchorName = "Anchor_Left";
		[SerializeField] private string _rightHandAnchorName = "Anchor_Right";

		[Header("Shooting")]
		[SerializeField]
		private GameObject _bulletPrefab;
		[SerializeField]
		private Transform _gunEnd;
		[SerializeField]
		private int _gunForce;

		[Header("Animations")]
		[SerializeField] private Animator _armsAnimator;
		[SerializeField] private string _animShootTrigger = "Shoot";

		private WeaponData[] _slotWeapons;
		private WeaponType _currentWeapon;
		private CarriedObject _carriedObject;
		public Camera Camera => _fpsCamera;
		public CharacterController Controller => _controller;

		public Transform LeftHandAnchor { get; private set; }
		public Transform RightHandAnchor { get; private set; }

		private int _animShootTriggerHash;

		private CharacterController _controller;
		private float _headRotation;
		private Vector2 _groundMovement = Vector2.zero;
		private bool _isReloading;

		private Interactible _eTarget;


		private void Awake()
		{
			LeftHandAnchor = _skeletonRoot.transform.FindChildRecursive(_leftHandAnchorName);
			RightHandAnchor = _skeletonRoot.transform.FindChildRecursive(_rightHandAnchorName);

			_animShootTriggerHash = Animator.StringToHash(_animShootTrigger);
		}

		private void Start()
		{
			_playerInput = GetComponent<PlayerInput>();
			_controller = GetComponent<CharacterController>();
			Cursor.lockState = CursorLockMode.Locked;
			_slotWeapons = new WeaponData[]
			{
				null,
				new()
				{
					Info = _info._mainWeapon,
					AmmoInGun = _info._mainWeapon.MaxNbOfBullets,
					NbOfMagazines = _info.NbOfMagazine
				},
				null
			};
			_currentWeapon = WeaponType.Secondary;
			UpdateAmmoDisplay();
		}

		private void FixedUpdate()
		{
			if (IsGamePaused())
			{
				return; // We can't move if we are in the menu
			}

			if (PlayerManager.S.PressE != null)
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

		public void PickObject(WeaponInfo weapon)
		{
			if (_slotWeapons[(int)weapon.Type] != null)
			{
				// TODO: Throw weapon on the ground
			}
			_slotWeapons[(int)weapon.Type] = new()
			{
				Info = weapon,
				NbOfMagazines = 0,
				AmmoInGun = weapon.MaxNbOfBullets
			};
		}

		public void PickObject(GameObject obj)
		{
			_carriedObject = new(transform, obj);
		}

		private void UpdateAmmoDisplay()
		{
			if (PlayerManager.S.AmmoDisplay != null)
			{
				PlayerManager.S.AmmoDisplay.text = $"{_slotWeapons[(int)_currentWeapon].AmmoInGun} / {_slotWeapons[(int)_currentWeapon].NbOfMagazines}";
			}
		}

		private bool IsGamePaused() => GameStateManager.Paused;

		public void EarnMagazine()
		{
			_slotWeapons[(int)_currentWeapon].NbOfMagazines++;
			UpdateAmmoDisplay();
		}

		private IEnumerator Reload()
		{
			_isReloading = true;
			yield return new WaitForSeconds(_slotWeapons[(int)_currentWeapon].Info.ReloadDuration);
			_slotWeapons[(int)_currentWeapon].AmmoInGun = _slotWeapons[(int)_currentWeapon].Info.MaxNbOfBullets;
			_slotWeapons[(int)_currentWeapon].NbOfMagazines--;
			UpdateAmmoDisplay();
			_isReloading = false;

		}

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

				_headRotation -= rot.y * _verticalLookMultiplier;
				_headRotation = Mathf.Clamp(_headRotation, -89, 89);
				_head.transform.localRotation = Quaternion.AngleAxis(_headRotation, Vector3.right);
			}
		}

		public void OnShoot(InputAction.CallbackContext value)
		{
			if (value.phase == InputActionPhase.Performed && // Already done the action when the button is pressed
				!IsGamePaused() && // Don't shoot if the game is paused
				!_isReloading && // We can't do it if we are reloading
				_slotWeapons[(int)_currentWeapon].AmmoInGun > 0 && // We can't do it if there are no ammo left
				_carriedObject == null) // Can't do anything while we are carrying an object
			{
				var go = Instantiate(_bulletPrefab, _gunEnd.position, Quaternion.identity);
				go.GetComponent<Rigidbody>().AddForce(_gunEnd.forward * _gunForce, ForceMode.Impulse);
				Destroy(go, 2f);

				if (_armsAnimator != null)
				{
					_armsAnimator.SetTrigger(_animShootTriggerHash);
				}

				_slotWeapons[(int)_currentWeapon].AmmoInGun--;
				if (_slotWeapons[(int)_currentWeapon].AmmoInGun == 0 && _slotWeapons[(int)_currentWeapon].NbOfMagazines > 0)
				{
					StartCoroutine(Reload());
				}
				UpdateAmmoDisplay();
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
				_eTarget?.InvokeCallback(this);
			}
		}

		#endregion
	}
}
