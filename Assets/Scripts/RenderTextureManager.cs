using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureManager : MonoBehaviour
{
    // Set this reference to a GameObject that has a Renderer component,
    // and a material that displays a texure (such as the Default material).
    // A standard Cube or other primitive works for the purposes of this example.

    private RenderTexture destinationTexture;

    void Start()
    {
        // Create a new Texture2D with the width and height of the screen, and cache it for reuse
        destinationTexture = new RenderTexture(Screen.width, Screen.height, 24);

        // Make screenGrabRenderer display the texture.
        Camera cam = GetComponent<Camera>();
        cam.targetTexture = destinationTexture;

        // Add the onPostRender callback
        Camera.onPostRender += OnPostRenderCallback;
    }

    void OnPostRenderCallback(Camera cam)
    {
        // Check whether the Camera that has just finished rendering is the one you want to take a screen grab from
        if (cam == Camera.main)
        {
            // Define the parameters for the ReadPixels operation
            Rect regionToReadFrom = new Rect(0, 0, Screen.width, Screen.height);
            int xPosToWriteTo = 0;
            int yPosToWriteTo = 0;
            bool updateMipMapsAutomatically = false;

            // Copy the pixels from the Camera's render target to the texture
            Texture2D temp = toTexture2D(destinationTexture);
            temp.ReadPixels(regionToReadFrom, xPosToWriteTo, yPosToWriteTo, updateMipMapsAutomatically);


            // Upload texture data to the GPU, so the GPU renders the updated texture
            // Note: This method is costly, and you should call it only when you need to
            // If you do not intend to render the updated texture, there is no need to call this method at this point
            temp.Apply();

            // Show the now updated texture on the screen
            GetComponent<Renderer>().material.mainTexture = temp;
            
            //destinationTexture = toRenderTexture(temp);

        }
    }

// Remove the onPostRender callback
    void OnDestroy()
    {
        Camera.onPostRender -= OnPostRenderCallback;
    }
    
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    
    private RenderTexture toRenderTexture(Texture2D temp)
    {
        RenderTexture rTex = new RenderTexture(temp.width, temp.height, 24);
        Graphics.Blit(temp, rTex);
        return rTex;
    }
}
