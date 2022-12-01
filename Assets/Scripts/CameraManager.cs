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
        RenderTexture leftRT = new RenderTexture(Screen.width, Screen.height, 24);
        leftRT.Create();
        
        RenderTexture rightRT = new RenderTexture(Screen.width, Screen.height, 24);
        rightRT.Create();

        cameraLeft.targetTexture = leftRT;
        cameraRight.targetTexture = rightRT;
        
        imageLeft.texture = leftRT;
        imageRight.texture = rightRT;

        RenderPipelineManager.beginFrameRendering += RenderScreen;
    }

    private void RenderScreen(ScriptableRenderContext context, Camera[] camera)
    {
        UniversalRenderPipeline.RenderSingleCamera(context, cameraLeft);
        UniversalRenderPipeline.RenderSingleCamera(context, cameraRight);
    }
}