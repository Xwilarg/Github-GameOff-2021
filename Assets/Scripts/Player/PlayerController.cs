using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
	private bool onGround = false;
	private Vector3 cummulativeInput = new Vector3(0, 0, 0);
	// where do I get the input from?
	[SerializeField]
	private PlayerInput playerInput;
	// get the parent transform
	// get the parent rigidbody
	
    void Start()
    { // Initialize? Maybe in awake?
        
    }
	
    void Update()
    { // Check input, cummulate vector until fixed update
        
    }
	
    void FixedUpdate()
    {
        // Check that player is on the ground
		// rotate cummulativeInput to align with the direction the player is facing
		// Apply rotated cummulativeInput to the rigidbody
		cummulativeInput.Set(0, 0, 0);
		
    }
	
	public void OnMovement(InputAction.CallbackContext value)
	{
		Vector2 inputMovement = value.ReadValue<Vector2>().normalized;
		cummulativeInput.x += inputMovement.x;
		cummulativeInput.z += inputMovement.y;
	}
}
