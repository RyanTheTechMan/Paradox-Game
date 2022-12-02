using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public Camera cameraLeft;
    public Camera cameraRight;
    
    public RawImage imageLeft;
    public RawImage imageRight;
    
    public Camera cameraMain;

    public Canvas screenResizingMessage;
    
    private RenderTexture _renderTextureLeft;
    private RenderTexture _renderTextureRight;
    
    private Transform _playerTransform; // Used for onDisable
    private Vector2 _screenSize; // Used for onDisable
    void Awake()
    {
        // Create a new texture for where the left camera will render.
        if (_renderTextureLeft != null) _renderTextureLeft.Release();
        _renderTextureLeft = new RenderTexture(Screen.width, Screen.height, 24);
        _renderTextureLeft.Create();
        
        // Create a new texture for where the right camera will render.
        if (_renderTextureRight != null) _renderTextureRight.Release();
        _renderTextureRight = new RenderTexture(Screen.width, Screen.height, 24);
        _renderTextureRight.Create();

        // Assign the left camera's target texture to the left texture.
        cameraLeft.targetTexture = _renderTextureLeft;
        // Assign the right camera's target texture to the right texture.
        cameraRight.targetTexture = _renderTextureRight;
        
        // Assign the left texture to render on the left of the screen.
        imageLeft.texture = _renderTextureLeft;
        // Assign the right texture to render on the right of the screen.
        imageRight.texture = _renderTextureRight;
        
        _screenSize.x = Screen.width;
        _screenSize.y = Screen.height;
    }

    private void Update()
    {
        // When the resolution changes, call OnResolutionChanged.
        if (_screenSize.x != Screen.width || _screenSize.y != Screen.height) OnResolutionChanged();

        _screenSize.x = Screen.width;
        _screenSize.y = Screen.height;
    }

    private void OnEnable()
    {
        // Before rendering the main camera, render the left and right cameras.
        RenderPipelineManager.beginFrameRendering += RenderScreen;

        // Set both colors to white so the image is visible on the screen.
        // (hidden by default so that we can see the scene in the editor)
        imageLeft.color = Color.white;
        imageRight.color = Color.white;
        
        // Move the main camera to the camera manager's child. 
        // (Main camera is attached to player by default so we can see the player prospective in the editor)
        _playerTransform = cameraMain.transform.parent; // Save the player transform so we can move it back on disable.
        cameraMain.transform.SetParent(transform);
        cameraMain.transform.localPosition = Vector3.zero;
    }
    
    private void OnDisable()
    {
        // Stop rendering the left and right cameras before rendering the main camera.
        RenderPipelineManager.beginFrameRendering -= RenderScreen;

        // Set both colors to clear so the image is hidden on the screen.
        // (hidden by default so that we can see the scene in the editor)
        imageLeft.color = Color.clear;
        imageRight.color = Color.clear;
        
        // Move the main camera back to the player.
        cameraMain.transform.SetParent(_playerTransform);
        cameraMain.transform.localPosition = Vector3.zero;
    }

    private void RenderScreen(ScriptableRenderContext context, Camera[] camera)
    {
        UniversalRenderPipeline.RenderSingleCamera(context, cameraLeft);
        UniversalRenderPipeline.RenderSingleCamera(context, cameraRight);
    }
    
    private void OnResolutionChanged()
    {
        if (screenResizingMessage.gameObject.activeSelf) return;
        StartCoroutine(WaitForResize());
    }

    private IEnumerator WaitForResize()
    {
        // Display a loading screen while we wait for the resize to finish.
        screenResizingMessage.gameObject.SetActive(true);
        
        // Is the screen still resizing?
        while (Screen.width != _screenSize.x || Screen.height != _screenSize.y)
        {
            yield return new WaitForSeconds(0.25f);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.25f);
        }
        
        // Regenerate the render textures.
        Awake();
        
        // Hide the loading screen.
        screenResizingMessage.gameObject.SetActive(false);
    }
}