using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {
    public static PlayerController Instance;
    
    public new Camera camera;
    [NonSerialized] public CharacterController characterController;
    [NonSerialized] public HandheldPortal handheldPortal;
    [NonSerialized] public int playerLayer;

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
    [NonSerialized] public MovableObject holdingObject;

    private void OnEnable() {
        controls.Enable();
    }
    
    private void OnDisable() {
        controls.Disable();
    }

    void Awake() {
        if (Instance == null) Instance = this;
        else {
            Debug.LogWarning("More than one instance of PlayerController found! Destroying this one.");
            Destroy(gameObject);
            return;
        }
        controls = new PlayerControls();
        
        characterController = GetComponent<CharacterController>();
        handheldPortal = GetComponentInChildren<HandheldPortal>();
        
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Left Eye"), LayerMask.NameToLayer("Right Eye"));
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void Start() {
        controls.FirstPerson.Jump.performed += ctx => DoJump();
        controls.FirstPerson.PrimaryInteract.performed += ctx => DoInteract(true);
        controls.FirstPerson.SecondaryInteract.performed += ctx => DoInteract(false);
        
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
        isGrounded = characterController.isGrounded;
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0;
        }

        Vector2 input = controls.FirstPerson.Move.ReadValue<Vector2>();
        
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        playerVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;
        characterController.Move((move + playerVelocity) * (playerSpeed * Time.fixedDeltaTime));
    }
    
    private void DoCameraMove() {
        Vector2 mouseDelta = controls.FirstPerson.Look.ReadValue<Vector2>();

        float mouseX = mouseDelta.x * cameraSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * cameraSensitivity * Time.deltaTime;
    
        transform.transform.localRotation *= Quaternion.Euler(0f, mouseX, 0f);
        camera.transform.localRotation *= Quaternion.AngleAxis(mouseY, Vector3.left);

        // transform.Rotate(Vector3.up, mouseX, Space.Self);
        // camera.transform.Rotate(Vector3.left, mouseY, Space.Self);
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

    private void DoInteract(bool isPrimary) {
        if (CursorController.Selected) {
            if (isPrimary) CursorController.Selected.PrimaryInteract();
            else CursorController.Selected.SecondaryInteract();
        }
    }
}
