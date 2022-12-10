using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HandheldPortal : MonoBehaviour {
    private Camera _camera;
    
    private RenderTexture _renderTexture;
    public RawImage cameraView;

    private PlayerController _playerController;
    
    public Volume volume;
    private PaniniProjection _paniniProjection;
    private LensDistortion _lensDistortion;

    [NonSerialized]
    public bool isPortalActive;

    private const float rotationUp = 0f;
    private const float rotationDown = 45f;
    private const float moveDownDistance = 1f;
    private float distanceMoved = 0f;
    private float basePositionY;
    
    private int _layerLeftEye;
    private int _layerPlayer;
    private int _layerDefault;
    [NonSerialized] public int nonInteractableLayer;

    private void OnEnable() {
        ResolutionChangeEvent.onResolutionChangedEnded += Awake;
        _camera.enabled = true;
        _playerController.controls.FirstPerson.TogglePortal.performed += TogglePortal;
        basePositionY = transform.localPosition.y;
    }

    private void OnDisable() {
        ResolutionChangeEvent.onResolutionChangedEnded -= Awake;
        _camera.enabled = true;
        _playerController.controls.FirstPerson.TogglePortal.performed -= TogglePortal;
    }

    private void Awake() {
        _camera = GetComponentInChildren<Camera>(true);
        _camera.gameObject.SetActive(true); // Disabled by default to fix issues with looking at game in the editor
        _playerController = PlayerController.Instance;
        
        _renderTexture?.Release();
        _renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _renderTexture.Create();

        _camera.targetTexture = _renderTexture;
        cameraView.texture = _renderTexture;

        Vector3 scale = cameraView.transform.localScale;
        float aspectRatio = (float)Screen.width / Screen.height;
        scale.x = scale.y * aspectRatio;
        cameraView.transform.localScale = scale;

        volume.profile.TryGet(out PaniniProjection paniniProjection);
        _paniniProjection = paniniProjection;
        
        volume.profile.TryGet(out LensDistortion lensDistortion);
        _lensDistortion = lensDistortion;

        _layerLeftEye = LayerMask.NameToLayer("Left Eye");
        _layerPlayer = LayerMask.NameToLayer("Player");
        _layerDefault = LayerMask.NameToLayer("Default");
    }

    private void Update() {
        // Apply post processing
        _paniniProjection.distance.value = Mathf.Sin(Time.time / 4f) * 0.1f + 0.3f;
        _lensDistortion.intensity.value = Mathf.Sin((Time.time - 3f)/3) * 0.35f;
        _lensDistortion.xMultiplier.value = Mathf.Cos((Time.time + 1f)/5) * 0.3f + 0.5f;
        _lensDistortion.yMultiplier.value = Mathf.Cos((Time.time - 8f)/6) * 0.3f + 0.5f;
    }

    private void FixedUpdate() {
        if (isPortalActive) _camera.transform.position = _playerController.camera.transform.position;

        _camera.fieldOfView = PlayerController.Instance.camera.fieldOfView * Mathf.Pow(-_camera.transform.localPosition.z, -1);

        // Move portal up or down if isPortalActive
        Vector3 rot = transform.localRotation.eulerAngles;
        rot.z = Mathf.Lerp(rot.z, isPortalActive ? rotationUp : rotationDown, Time.fixedUnscaledDeltaTime * 1f);
        transform.localRotation = Quaternion.Euler(rot);
        distanceMoved = Mathf.Lerp(distanceMoved, isPortalActive ? 0f : moveDownDistance, Time.fixedUnscaledDeltaTime * 2f);

        distanceMoved = Mathf.Lerp(distanceMoved, isPortalActive ? 0f : moveDownDistance, Time.fixedUnscaledDeltaTime * 2f);
        transform.localPosition = new Vector3(transform.localPosition.x, basePositionY - distanceMoved, transform.localPosition.z);

        foreach (Transform child in transform) { // Show or hide portal components when portal is shown or hidden
            child.gameObject.SetActive(transform.localPosition.y > -1.4f);
        }
    }
    
    private void TogglePortal(InputAction.CallbackContext obj) {
        isPortalActive = !isPortalActive;
        
        Physics.IgnoreLayerCollision(_layerLeftEye, _layerDefault, !isPortalActive);
        Physics.IgnoreLayerCollision(_layerLeftEye, _layerPlayer, !isPortalActive);
        nonInteractableLayer = isPortalActive ? -1 : _layerLeftEye;
        
        if (PlayerController.Instance.holdingObject) {
            PlayerController.Instance.holdingObject.Drop();
        }
    }
}
