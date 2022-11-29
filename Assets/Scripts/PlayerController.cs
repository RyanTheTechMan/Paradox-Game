using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

public class PlayerController : MonoBehaviour {
    public GameObject playerCamera;
    
    public float playerSpeed = 2.0f;
    public float jumpForce = 50.0f;
    // public float jumpHeight = 1.0f;
    // public float gravityValue = -9.81f;

    private Rigidbody rigidbody;
    private Vector3 playerVelocity;
    private bool isGrounded;

    private float horizontalInput;
    private float verticalInput;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * verticalInput) + (right * horizontalInput);
        playerVelocity = moveDirection * playerSpeed;

        if (Input.GetButtonDown("Jump")) {
            rigidbody.AddForce(Vector3.up * jumpForce);
        }

        transform.Translate(playerVelocity * Time.deltaTime);
    }
}
