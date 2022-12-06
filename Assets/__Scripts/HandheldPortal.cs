using System;
using UnityEngine;
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

    private void OnEnable()
    {
        ResolutionChangeEvent.onResolutionChangedEnded += Awake;
        _camera.enabled = true;
    }
    
    
    private void OnDisable()
    {
        ResolutionChangeEvent.onResolutionChangedEnded -= Awake;
        _camera.enabled = true;
    }

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        
        if (_renderTexture != null) _renderTexture.Release();
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
        _paniniProjection.distance.value = Mathf.Sin(Time.time/4f) * 0.1f + 0.3f;
        
        _lensDistortion.intensity.value = Mathf.Sin((Time.time - 3f)/3) * 0.35f;
        _lensDistortion.xMultiplier.value = Mathf.Cos((Time.time + 1f)/5) * 0.3f + 0.5f;
        _lensDistortion.yMultiplier.value = Mathf.Cos((Time.time - 8f)/6) * 0.3f + 0.5f;
    }

    private void FixedUpdate()
    {
        _camera.transform.position = _player._camera.transform.position;
        _camera.transform.rotation = _player._camera.transform.rotation;
    }
}
