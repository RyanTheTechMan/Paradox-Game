using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

public class PlayerController : MonoBehaviour {
    public GameObject camera;
    private CharacterController controller;
    
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    
    private Vector3 playerVelocity;
    private bool isGrounded;

    private float horizontalInput;
    private float verticalInput;
    
    void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * verticalInput) + (right * horizontalInput);
        playerVelocity = moveDirection * playerSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
