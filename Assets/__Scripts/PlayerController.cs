using System;
using UnityEngine;

// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

public class PlayerController : MonoBehaviour {
    public Camera _camera;
    private CharacterController _characterController;

    public float cameraSensitivity;
    public float playerSpeed;
    public float jumpForce;
    public float pushForce;
    public float throwForce;

    private Vector3 playerVelocity;
    private bool isGrounded;

    [NonSerialized]
    public PlayerControls controls;

    public Rigidbody hand;

    private void OnEnable() {
        controls.Enable();
    }
    
    private void OnDisable() {
        controls.Disable();
    }

    void Awake() {
        _characterController = GetComponent<CharacterController>();
        controls = new PlayerControls();
    }

    private void Start() {
        controls.FirstPerson.Jump.performed += ctx => DoJump();
        controls.FirstPerson.PrimaryInteract.performed += ctx => DoRaycast(true);
        controls.FirstPerson.SecondaryInteract.performed += ctx => DoRaycast(false);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update() {
    }

    private void FixedUpdate() {
        DoPlayerMove();
        DoCameraMove();
    }

    private void DoPlayerMove() {
        isGrounded = _characterController.isGrounded;
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0;
        }

        Vector2 input = controls.FirstPerson.Move.ReadValue<Vector2>();
        
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        playerVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;
        _characterController.Move((move + playerVelocity) * (playerSpeed * Time.fixedDeltaTime));
    }
    
    private void DoCameraMove() {
        Vector2 mouseDelta = controls.FirstPerson.Look.ReadValue<Vector2>();

        float mouseX = mouseDelta.x * cameraSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * cameraSensitivity * Time.deltaTime;
        
        transform.Rotate(Vector3.up, mouseX, Space.Self);
        _camera.transform.Rotate(Vector3.left, mouseY, Space.Self);

        // Clamp camera rotation
        //float xRotation = _camera.transform.localRotation.eulerAngles.x;
        //if (xRotation > 180) xRotation -= 360;
        //xRotation = Mathf.Clamp(xRotation, -90, 90);
        //_camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    private void DoJump() {
        if (isGrounded) {
            playerVelocity.y += Mathf.Sqrt(-jumpForce * Physics.gravity.y);
        }
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        MovableObject interactable = hit.gameObject.GetComponent<MovableObject>();
        if (interactable && interactable.canPush) {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null && !rb.isKinematic) {
                rb.velocity = hit.moveDirection * pushForce + new Vector3(0, -0.2f, 0);
            }
        }
    }

    private void DoRaycast(bool isPrimary) {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit)) {
            InteractableObject interactable = hit.transform.gameObject.GetComponent<InteractableObject>();
            if (interactable && interactable.CanInteract(_characterController.transform)) {
                if (isPrimary) interactable.PrimaryInteract();
                else interactable.SecondaryInteract();
            }
        }
    }
}
