using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }
    public static event Action OnLevelLoad;
    public static event Action OnLevelUnload;
    public static event Action OnLevelComplete;

    public Vector3 PlayerSpawnPosition;

    [SerializeField] private GameObject levelLoadRoom;
    
    private static List<Scene> _levels;
    
    private int _levelIndex = 0;

    public void Awake() {
        // If we don't currently know all levels, find them
        if (_levels == null) {
            _levels = new List<Scene>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                int lastSlash = path.LastIndexOf("/");
                string sceneName = path.Substring(lastSlash + 1, path.LastIndexOf(".") - lastSlash - 1);
                if (sceneName.StartsWith("level")) {
                    Debug.Log("Adding \"" + sceneName + "\" to level list.");
                    _levels.Add(SceneManager.GetSceneByName(sceneName));
                }
            }
        }
        
        // If LevelManager is not already set, set it. One per level.
        if (Instance == null) Instance = this;
        else {
            Destroy(gameObject);
            Debug.LogWarning("LevelManager already exists. Destroying duplicate.");
        }
        OnLevelLoad?.Invoke();
        
        // unload async scene
        //StartCoroutine(UnloadAsyncScene(_levels[_levelIndex]));
    }

    private IEnumerator LoadAsyncScene(Scene scene) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_levels[_levelIndex].name, LoadSceneMode.Additive);
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
    
    private IEnumerator UnloadAsyncScene(Scene scene) {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_levels[_levelIndex], UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        while (!asyncUnload.isDone) {
            yield return null;
        }
    }

    public void OnLevelEnd() {
        Debug.LogWarning("Number 1 victory royale");
    }
}
