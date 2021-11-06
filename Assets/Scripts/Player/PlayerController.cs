using Bug.Prop;
using Bug.SO;
using Bug.Weapon;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bug.Player
{
	public class PlayerController : MonoBehaviour
	{
		public PlayerInfo _info;

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

		private WeaponData _mainWeapon;
		private CharacterController _controller;
		private Vector2 _groundMovement = Vector2.zero;
		private bool _isReloading;

		private Interactible _eTarget;
		
		private void Start()
		{
			_playerInput = GetComponent<PlayerInput>();
			_controller = GetComponent<CharacterController>();
			Cursor.lockState = CursorLockMode.Locked;
			_mainWeapon = new()
			{
				Info = _info._mainWeapon,
				AmmoInGun = _info._mainWeapon.MaxNbOfBullets,
				NbOfMagazines = _info.NbOfMagazine
			};
			UpdateAmmoDisplay();
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

		private void UpdateAmmoDisplay()
        {
			PlayerManager.S.AmmoDisplay.text = $"{_mainWeapon.AmmoInGun} / {_mainWeapon.NbOfMagazines}";
        }

		private bool IsGamePaused()
			=> PlayerManager.S.PauseMenu.IsActive();

		public void EarnMagazine()
        {
			_mainWeapon.NbOfMagazines++;
			UpdateAmmoDisplay();
        }

		private IEnumerator Reload()
        {
			_isReloading = true;
			yield return new WaitForSeconds(_mainWeapon.Info.ReloadDuration);
			_mainWeapon.AmmoInGun = _mainWeapon.Info.MaxNbOfBullets;
			_mainWeapon.NbOfMagazines--;
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
				transform.rotation *= Quaternion.Euler(0f, rot.x * _horizontalLookMultiplier, 0f);
				_fpsCamera.transform.rotation *= Quaternion.Euler(rot.y * _verticalLookMultiplier, 0f, 0f);
			}
		}

		public void OnShoot(InputAction.CallbackContext value)
        {
			if (value.phase == InputActionPhase.Performed && // Already done the action when the button is pressed
				!IsGamePaused() && // Don't shoot if the game is paused
				!_isReloading && // We can't do it if we are reloading
				_mainWeapon.AmmoInGun > 0) // We can't do it if there are no ammo left
            {
				var go = Instantiate(_bulletPrefab, _gunEnd.position, Quaternion.identity);
				go.GetComponent<Rigidbody>().AddForce(_gunEnd.forward * _gunForce, ForceMode.Impulse);
				Destroy(go, 2f);

				_mainWeapon.AmmoInGun--;
				if (_mainWeapon.AmmoInGun == 0 && _mainWeapon.NbOfMagazines > 0)
                {
					StartCoroutine(Reload());
                }
				UpdateAmmoDisplay();
			}
        }

		public void OnMenu(InputAction.CallbackContext _)
        {
			PlayerManager.S.PauseMenu.Toggle();
			Cursor.lockState = PlayerManager.S.PauseMenu.IsActive() ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = PlayerManager.S.PauseMenu.IsActive();
        }

		public void OnAction(InputAction.CallbackContext _)
        {
			_eTarget?.InvokeCallback(this);
		}

        #endregion
    }
}