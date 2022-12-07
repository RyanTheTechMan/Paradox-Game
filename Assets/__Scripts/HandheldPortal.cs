using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HandheldPortal : MonoBehaviour
{
    private Camera _camera;
    
    private RenderTexture _renderTexture;
    public RawImage cameraView;

    private PlayerController _player;
    
    public Volume volume;
    private PaniniProjection _paniniProjection;
    private LensDistortion _lensDistortion;
    
    private bool _isPortalActive;

    private const float rotationUp = 0f;
    private const float rotationDown = 45f;
    private const float moveDownDistance = 1f;
    private float distanceMoved = 0f;
    private float basePositionY;

    private void OnEnable()
    {
        ResolutionChangeEvent.onResolutionChangedEnded += Awake;
        _camera.enabled = true;
        _player.controls.FirstPerson.TogglePortal.performed += TogglePortal;
        basePositionY = transform.localPosition.y;
    }

    private void OnDisable() {
        ResolutionChangeEvent.onResolutionChangedEnded -= Awake;
        _camera.enabled = true;
        _player.controls.FirstPerson.TogglePortal.performed -= TogglePortal;
    }

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        
        _renderTexture?.Release();
        _renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _renderTexture.Create();
        
        _player ??= GameObject.FindWithTag("Player").GetComponent<PlayerController>();

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
    }

    private void Update() {
        
        // Apply post processing
        _paniniProjection.distance.value = Mathf.Sin(Time.time / 4f) * 0.1f + 0.3f;
        _lensDistortion.intensity.value = Mathf.Sin((Time.time - 3f)/3) * 0.35f;
        _lensDistortion.xMultiplier.value = Mathf.Cos((Time.time + 1f)/5) * 0.3f + 0.5f;
        _lensDistortion.yMultiplier.value = Mathf.Cos((Time.time - 8f)/6) * 0.3f + 0.5f;
    }

    private void FixedUpdate()
    {
        if (_isPortalActive) _camera.transform.position = _player._camera.transform.position;

        // Move portal up or down if _isPortalActive
        Vector3 rot = transform.localRotation.eulerAngles;
        rot.z = Mathf.Lerp(rot.z, _isPortalActive ? rotationUp : rotationDown, Time.fixedUnscaledDeltaTime * 1f);
        transform.localRotation = Quaternion.Euler(rot);
        distanceMoved = Mathf.Lerp(distanceMoved, _isPortalActive ? 0f : moveDownDistance, Time.fixedUnscaledDeltaTime * 1.5f);

        distanceMoved = Mathf.Lerp(distanceMoved, _isPortalActive ? 0f : moveDownDistance, Time.fixedUnscaledDeltaTime * 1.5f);
        transform.localPosition = new Vector3(transform.localPosition.x, basePositionY - distanceMoved, transform.localPosition.z);
    }
    
    private void TogglePortal(InputAction.CallbackContext obj) {
        _isPortalActive = !_isPortalActive;
    }

}
