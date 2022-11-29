using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public Camera cameraLeft;
    public Camera cameraRight;
    public Camera cameraMain;
    private RawImage mainImage;
    void Start()
    {
        RawImage raw = cameraMain.GetComponent<RawImage>();
        mainImage = raw;
        
        RenderTexture leftRT = new RenderTexture(Screen.width, Screen.height, 24);
        leftRT.Create();
        
        RenderTexture rightRT = new RenderTexture(Screen.width, Screen.height, 24);
        rightRT.Create();

        cameraLeft.targetTexture = leftRT;
        cameraRight.targetTexture = rightRT;
    }

    void Update()
    {
        Texture2D left = RTImage(cameraLeft);
        Texture2D right = RTImage(cameraRight);
        
        Texture2D merged = new Texture2D(left.width, left.height);
        
        for (int i = 0; i < left.width; i++)
        {
            for (int j = 0; j < left.height; j++)
            {
                merged.SetPixel(i, j, left.width / 2 > i ? left.GetPixel(i, j) : right.GetPixel(i, j));
            }
        }
        
        merged.Apply();
        mainImage.texture = merged;
    }
    
    // Take a "screenshot" of a camera's Render Texture.
    Texture2D RTImage(Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        return image;
    }
}

/*
IDEA TO SPLIT THE SCREEN SAVE PROCESSING - NOT WOKRING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public Camera cameraLeft;
    public Camera cameraRight;
    public Camera cameraMain;
    private RawImage mainImage;
    void Start()
    {
        RawImage raw = cameraMain.GetComponent<RawImage>();
        mainImage = raw;
        
        RenderTexture leftRT = new RenderTexture(Screen.width, Screen.height, 24);
        leftRT.Create();
        
        RenderTexture rightRT = new RenderTexture(Screen.width, Screen.height, 24);
        rightRT.Create();

        cameraLeft.targetTexture = leftRT;
        cameraRight.targetTexture = rightRT;
    }

    void Update()
    {
        Texture2D left = RTImage(cameraLeft, true);
        Texture2D right = RTImage(cameraRight, false);
        
        Texture2D merged = new Texture2D(left.width*2, left.height);
        
        for (int i = 0; i < left.width*2; i++)
        {
            for (int j = 0; j < left.height; j++)
            {
                if (i < left.width)
                {
                    merged.SetPixel(i, j, left.GetPixel(i, j));
                }
                else
                {
                    merged.SetPixel(i, j, right.GetPixel(i - left.width, j));
                }
            }
        }
        
        merged.Apply();
        
        mainImage.texture = left;
    }
    
    // Take a "screenshot" of a camera's Render Texture.
    Texture2D RTImage(Camera camera, bool left)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        
        if (left)
            image.ReadPixels(new Rect(0, 0, camera.targetTexture.width/2, camera.targetTexture.height), 0, 0);
        else 
            image.ReadPixels(new Rect(camera.targetTexture.width/2, 0, camera.targetTexture.width/2, camera.targetTexture.height), 0, 0);
        
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        return image;
    }
}
*/