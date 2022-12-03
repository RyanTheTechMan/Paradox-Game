using System;
using UnityEngine;
using UnityEngine.UI;

public class HandheldPortal : MonoBehaviour
{
    private Camera _camera;
    
    private RenderTexture _renderTexture;
    public RawImage cameraView;

    private PlayerController _player;

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
    }

    private void FixedUpdate()
    {
        _camera.transform.position = _player._camera.transform.position;
        _camera.transform.rotation = _player._camera.transform.rotation;
    }
}
