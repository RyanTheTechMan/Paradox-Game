using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

public class PlayerController : MonoBehaviour {
    public Camera cameraLeft;
    public Camera cameraRight;
    
    public float playerSpeed = 2.0f;
    public float jumpForce = 50.0f;
    // public float jumpHeight = 1.0f;
    // public float gravityValue = -9.81f;

    private Rigidbody rigidbody;
    private Vector3 playerVelocity;
    private bool isGrounded;

    private PlayerControls controls;

    private void OnEnable() {
        controls.Enable();
    }
    
    private void OnDisable() {
        controls.Disable();
    }

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        controls = new PlayerControls();
    }

    private void Start() {
        controls.FirstPerson.Jump.performed += ctx => DoJump();
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update() {
        DoPlayerMove();
    }

    private void FixedUpdate() {
        DoCameraMove();
    }

    private void DoPlayerMove() {
        float horizontalInput = controls.FirstPerson.Move.ReadValue<Vector2>().x;
        float verticalInput = controls.FirstPerson.Move.ReadValue<Vector2>().y;
        
        Vector3 forward = cameraLeft.transform.forward;
        Vector3 right = cameraLeft.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * verticalInput) + (right * horizontalInput);
        playerVelocity = moveDirection * playerSpeed;

        transform.Translate(playerVelocity * Time.deltaTime);
    }
    
    private void DoCameraMove() {
        Vector2 mouseDelta = controls.FirstPerson.Look.ReadValue<Vector2>();
        
        cameraLeft.transform.rotation *= Quaternion.AngleAxis(mouseDelta.x * Time.deltaTime * 30, Vector3.up);
        cameraLeft.transform.rotation *= Quaternion.AngleAxis(mouseDelta.y * Time.deltaTime * 30, Vector3.left);
        cameraLeft.transform.rotation = Quaternion.Euler(cameraLeft.transform.rotation.eulerAngles.x, cameraLeft.transform.rotation.eulerAngles.y, 0);
        
        cameraRight.transform.rotation = cameraLeft.transform.rotation;
    }

    private void DoJump() {
        rigidbody.AddForce(Vector3.up * jumpForce);
    }
}
