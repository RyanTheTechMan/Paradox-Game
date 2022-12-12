using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    private Camera mainCamera;
    public float startZ;
    public float endZ;
    public float beginZ = 45;
    public float mainMenuZ = -8.75f;
    public float levelSelectZ = 7.5f;
    public float speed;

    private DepthOfField dof;
    private LensDistortion distortion;
    private ColorAdjustments colorAdjustments;
    
    public Volume volumeBlur;
    
    private static List<string> _levels;
    public Canvas levelSelectContainer;
    public Canvas levelSelectFrame;
    public GameObject levelButtonPrefab;

    public CanvasGroup levelSelectMask;
    private float _maskTrans;
    
    private void Awake() {
        if (_levels == null) {
            _levels = new List<string>();
            _levels.Add(""); // Create a dummy scene so ID's line up with level numbers.
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                int lastSlash = path.LastIndexOf("/");
                string sceneName = path.Substring(lastSlash + 1, path.LastIndexOf(".") - lastSlash - 1);
                if (sceneName.StartsWith("level")) {
                    _levels.Add(sceneName);

                    GameObject button =  Instantiate(levelButtonPrefab, levelSelectFrame.transform);
                    button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level " + (i - 1);
                }
            }
        }
        
        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, startZ);
        
        volumeBlur.profile.TryGet(out DepthOfField depthOfField);
        dof = depthOfField;
        
        volumeBlur.profile.TryGet(out LensDistortion lensDistortion);
        distortion = lensDistortion;
        
        volumeBlur.profile.TryGet(out ColorAdjustments colorAdjustments1);
        colorAdjustments = colorAdjustments1;

        levelSelectMask.alpha = 0;
    }

    public void PlayButton() {
        // float temp = startZ;
        // startZ = endZ;
        // endZ = temp / 2;

        endZ = startZ;

        StartCoroutine(LoadLevelLoader());
    }

    public void QuitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void LevelSelectButton() {
        _maskTrans = 1;
        endZ = levelSelectZ;
        StartCoroutine(DoSelectTransparency());
    }

    public void MainMenuButton() {
        _maskTrans = 0;
        StartCoroutine(DoSelectTransparency());
        endZ = mainMenuZ;
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

    private IEnumerator DoSelectTransparency() {
        float startPos = levelSelectMask.alpha;
        float endPos = _maskTrans;
        
        float t = 0f;
        float animateState = endPos;
        while (t < 1f && Mathf.Approximately(animateState, _maskTrans)) {
            t += Time.deltaTime * 0.75f;
            levelSelectMask.alpha = Mathf.Lerp(startPos, endPos, t);
            yield return null;
        }
    }
}
