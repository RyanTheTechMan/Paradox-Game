using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private Camera mainCamera;
    public float startZ;
    public float endZ;
    public float speed;

    private DepthOfField dof;
    private LensDistortion distortion;
    private ColorAdjustments colorAdjustments;
    
    public Volume volumeBlur;
    
    private void Awake() {
        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, startZ);
        
        volumeBlur.profile.TryGet(out DepthOfField depthOfField);
        dof = depthOfField;
        
        volumeBlur.profile.TryGet(out LensDistortion lensDistortion);
        distortion = lensDistortion;
        
        volumeBlur.profile.TryGet(out ColorAdjustments colorAdjustments1);
        colorAdjustments = colorAdjustments1;
    }

    public void PlayButton() {
        
        StartCoroutine(LoadLevelLoader());
    }

    public void QuitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private IEnumerator LoadLevelLoader() {
        // Dip screen to black
        for (float i = 0; i < 3; i += Time.deltaTime) {
            colorAdjustments.postExposure.value = Mathf.Lerp(0, -10, i);
            yield return null;
        }
        
        SceneManager.LoadScene("loader");
    }

    private void FixedUpdate() {
        float distance = Mathf.Abs(mainCamera.transform.position.z - startZ) / Mathf.Abs(endZ - startZ);
        
        dof.focusDistance.value = Mathf.Lerp(-1f, 1.62f, distance);
        
        // Move camera from startZ to endZ with lerp
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, Mathf.Lerp(mainCamera.transform.position.z, endZ, speed * Time.fixedDeltaTime));

        float x = (Input.mousePosition.x - Screen.width) / Screen.width;
        float y = (Input.mousePosition.y - Screen.height) / Screen.height;
        
        // push in camera by rotating based on mouse distance from center of screen
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, Quaternion.Euler(-y * 10, x * 10, 0), Time.fixedDeltaTime);

        Vector3 rot1 = mainCamera.transform.eulerAngles;
        rot1.z = 65;
        
        Vector3 rot2 = mainCamera.transform.eulerAngles;
        rot2.z = 0;

        mainCamera.transform.rotation = Quaternion.Lerp(Quaternion.Euler(rot1), Quaternion.Euler(rot2), distance);
        
        distortion.intensity.value = Mathf.Lerp(-0.9f, 0f, distance);
        distortion.center.value = new Vector2(Mathf.Lerp(0.7f,0.5f, distance), 0.5f);
        distortion.scale.value = Mathf.Lerp(0.4f, 1f, distance);
        
        colorAdjustments.postExposure.value = Mathf.Lerp(-10, 0f, distance*2f);
    }
}
