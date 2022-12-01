using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

public class PlayerController : MonoBehaviour {
    public Camera camera;

    public float cameraSensitivity = 15.0f;
    public float playerSpeed = 2.0f;
    public float jumpForce = 50.0f;

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
        // DoPlayerMove();
    }

    private void FixedUpdate() {
        DoCameraMove();
        DoPlayerMove();
    }

    private void DoPlayerMove() {
        float horizontalInput = controls.FirstPerson.Move.ReadValue<Vector2>().x;
        float verticalInput = controls.FirstPerson.Move.ReadValue<Vector2>().y;
        
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;

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
        
        camera.transform.rotation *= Quaternion.AngleAxis(mouseDelta.x * Time.deltaTime * cameraSensitivity, Vector3.up);
        camera.transform.rotation *= Quaternion.AngleAxis(mouseDelta.y * Time.deltaTime * cameraSensitivity, Vector3.left);
        camera.transform.rotation = Quaternion.Euler(camera.transform.rotation.eulerAngles.x, camera.transform.rotation.eulerAngles.y, 0);
    }

    private void DoJump() {
        rigidbody.AddForce(Vector3.up * jumpForce);
    }
}
