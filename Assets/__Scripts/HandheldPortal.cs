using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [NonSerialized] public bool isPortalActive;

    private const float rotationUp = 0f;
    private const float rotationDown = 45f;
    private const float moveDownDistance = 1f;
    private float distanceMoved = 0f;
    private float basePositionY;
    
    private int _layerLeftEye;
    private int _layerPlayer;
    private int _layerDefault;
    [NonSerialized] public int nonInteractableLayer;

    private AudioSource _audioSource;
    public float portalVolume = 0.15f;
    private float _volumeTimer = 0f;

    private void OnEnable() {
        ResolutionChangeEvent.onResolutionChangedEnded += Awake;
        _camera.enabled = true;
        _playerController.controls.FirstPerson.TogglePortal.performed += TogglePortal;
        
        OnPortalToggle();
    }

    private void OnDisable() {
        ResolutionChangeEvent.onResolutionChangedEnded -= Awake;
        _camera.enabled = true;
        _playerController.controls.FirstPerson.TogglePortal.performed -= TogglePortal;
    }

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
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

        basePositionY = transform.localPosition.y;
    }

    private void Update() {
        // Apply post processing
        _paniniProjection.distance.value = Mathf.Sin(Time.time / 4f) * 0.1f + 0.3f;
        _lensDistortion.intensity.value = Mathf.Sin((Time.time - 3f)/3) * 0.35f;
        _lensDistortion.xMultiplier.value = Mathf.Cos((Time.time + 1f)/5) * 0.3f + 0.5f;
        _lensDistortion.yMultiplier.value = Mathf.Cos((Time.time - 8f)/6) * 0.3f + 0.5f;

        _audioSource.panStereo = Mathf.Sin(_volumeTimer);

        _volumeTimer += Time.deltaTime * 0.2f;
    }

    private void FixedUpdate() {
        _camera.transform.position = _playerController.camera.transform.position;
        _camera.fieldOfView = PlayerController.Instance.camera.fieldOfView * Mathf.Pow(-_camera.transform.localPosition.z, -1);
    }
    
    private void TogglePortal(InputAction.CallbackContext obj) {
        // prevent opening portal if trying to and cannot open
        if (!LevelManager.Instance.CanUsePortal && !isPortalActive) return;
        
        isPortalActive = !isPortalActive;
        OnPortalToggle();
    }

    private void OnPortalToggle() {
        UpdateCollisions();
        StartCoroutine(DoAnimate());

        if (PlayerController.Instance.holdingObject) {
            PlayerController.Instance.holdingObject.Drop();
        }
    }

    public void ShowPortal() {
        if (isPortalActive || !LevelManager.Instance.CanUsePortal) return;
        isPortalActive = true;
        OnPortalToggle();
    }

    public void HidePortal() {
        if (!isPortalActive) return;
        isPortalActive = false;
        OnPortalToggle();
    }

    public void UpdateCollisions() {
        Physics.IgnoreLayerCollision(_layerLeftEye, _layerDefault, !isPortalActive);
        Physics.IgnoreLayerCollision(_layerLeftEye, _layerPlayer, !isPortalActive);
        nonInteractableLayer = isPortalActive ? -1 : _layerLeftEye;
    }

    private IEnumerator DoAnimate() {
        Vector3 startRot = transform.localRotation.eulerAngles;
        float targetRot = isPortalActive ? rotationUp : rotationDown;

        Vector3 startPos = transform.localPosition;
        float targetPos = isPortalActive ? basePositionY : basePositionY - moveDownDistance;
        
        float startVolume = _audioSource.volume;
        float endVolume = isPortalActive ? portalVolume : 0;

        float t = 0f;
        bool animateState = isPortalActive;
        while (t < 1f && animateState == isPortalActive) {
            t += Time.deltaTime * 1.5f;
            transform.localRotation = Quaternion.Euler(startRot.x, startRot.y, Mathf.Lerp(startRot.z, targetRot, t));
            transform.localPosition = new Vector3(startPos.x, Mathf.Lerp(startPos.y, targetPos, t), startPos.z);
            _audioSource.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }
    }
}
