using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    
    private const float _viewBobbingIntensity = 0.015f;
    private float _viewBobTimer = 0;

    public float cameraLock = 85f;
    private float _cameraRot = 0f;

    private Vector3 moveDirection;
    private Vector3 playerVelocity;
    private bool isGrounded;

    [NonSerialized] public PlayerControls controls;

    public Rigidbody hand; 
    [NonSerialized] public MovableObject holdingObject;
    
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    private bool shouldFootstep = false;

    private void OnEnable() {
        controls?.Enable();
    }
    
    private void OnDisable() {
        controls?.Disable();
    }

    void Awake() {
        if (Instance) {
            Debug.LogWarning("More than one instance of PlayerController found! Destroying this one.");
            gameObject.SetActive(false);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
        
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
    
    void Update() {}

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
        
        moveDirection = transform.right * input.x + transform.forward * input.y;
        playerVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;
        characterController.Move((moveDirection + playerVelocity) * (playerSpeed * Time.fixedDeltaTime));
    }
    
    private void DoCameraMove() {
        // get mouse input
        Vector2 mouseDelta = controls.FirstPerson.Look.ReadValue<Vector2>();
        float mouseX = mouseDelta.x * cameraSensitivity * Time.fixedDeltaTime;
        float mouseY = mouseDelta.y * cameraSensitivity * Time.fixedDeltaTime;
        
        // rotate player on x-axis
        transform.Rotate(Vector3.up, mouseX);

        // rotate camera on y-axis
        _cameraRot -= mouseY;
        _cameraRot = Mathf.Clamp(_cameraRot, -cameraLock, cameraLock);
        Vector3 targetRot = transform.eulerAngles;
        targetRot.x = _cameraRot;
        camera.transform.eulerAngles = targetRot;

        // walk cycle view bobbing
        Vector3 cameraPos = camera.transform.localPosition;
        Vector3 cameraRot = camera.transform.localRotation.eulerAngles;
        if (moveDirection.magnitude > 0.1f) { // is moving
            _viewBobTimer += playerSpeed * Time.fixedDeltaTime * 3;
            camera.transform.localPosition = new Vector3(cameraPos.x, Mathf.Sin(_viewBobTimer) * _viewBobbingIntensity, cameraPos.z);
            camera.transform.localRotation = Quaternion.Euler(cameraRot.x, cameraRot.y, Mathf.Sin(_viewBobTimer / 2) * _viewBobbingIntensity);

            float footfall = Mathf.Cos(_viewBobTimer);
            if (footfall < 0 && shouldFootstep && isGrounded) DoFootstepSound();

            shouldFootstep = footfall >= 0;
            
            if (_viewBobTimer > Math.PI * 2) {
                _viewBobTimer -= (float)Math.PI * 2;
            }
        }
        else { // is idle
            _viewBobTimer = 0;
            camera.transform.localPosition = new Vector3(cameraPos.x, Mathf.Lerp(cameraPos.y, 0, playerSpeed * _viewBobbingIntensity), cameraPos.z);
            camera.transform.localRotation = Quaternion.Euler(cameraRot.x, cameraRot.y, Mathf.Lerp(cameraPos.y, 0, playerSpeed * _viewBobbingIntensity));
        }
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

    private void DoFootstepSound() {
        AudioClip footstep = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepSource.pitch = Random.Range(0.5f, 0.6f);
        footstepSource.volume = Random.Range(0.8f, 1.2f);
        footstepSource.PlayOneShot(footstep);
    }
}
