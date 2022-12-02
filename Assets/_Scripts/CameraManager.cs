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
    void Awake()
    {
        // Create a new texture for where the left camera will render.
        RenderTexture leftRT = new RenderTexture(Screen.width, Screen.height, 24);
        leftRT.Create();
        
        // Create a new texture for where the right camera will render.
        RenderTexture rightRT = new RenderTexture(Screen.width, Screen.height, 24);
        rightRT.Create();

        // Assign the left camera's target texture to the left texture.
        cameraLeft.targetTexture = leftRT;
        // Assign the right camera's target texture to the right texture.
        cameraRight.targetTexture = rightRT;
        
        // Assign the left texture to render on the left of the screen.
        imageLeft.texture = leftRT;
        // Assign the right texture to render on the right of the screen.
        imageRight.texture = rightRT;
        
        // Before rendering the main camera, render the left and right cameras.
        RenderPipelineManager.beginFrameRendering += RenderScreen;

        // Set both colors to white so the image is visible on the screen.
        // (hidden by default so that we can see the scene in the editor)
        imageLeft.color = Color.white;
        imageRight.color = Color.white;
        
        // Move the main camera to the camera manager's child. 
        // (Main camera is attached to player by default so we can see the player prospective in the editor)
        cameraMain.transform.SetParent(transform);
        cameraMain.transform.localPosition = Vector3.zero;
    }

    private void RenderScreen(ScriptableRenderContext context, Camera[] camera)
    {
        UniversalRenderPipeline.RenderSingleCamera(context, cameraLeft);
        UniversalRenderPipeline.RenderSingleCamera(context, cameraRight);
    }
}