using UnityEngine;

// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

public class PlayerController : MonoBehaviour {
    public Camera _camera;
    private CharacterController _characterController;

    public float cameraSensitivity = 15.0f;
    public float playerSpeed = 2.0f;
    public float jumpForce = 50.0f;

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
        _characterController = GetComponent<CharacterController>();
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
        isGrounded = _characterController.isGrounded;
        if (isGrounded && playerVelocity.y < 0) playerVelocity.y = 0f;

        Vector2 input = controls.FirstPerson.Move.ReadValue<Vector2>();
        
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        _characterController.Move(move * playerSpeed * Time.deltaTime);

        playerVelocity.y += Physics.gravity.y * Time.deltaTime;
        _characterController.Move(playerVelocity * Time.deltaTime);
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
}
