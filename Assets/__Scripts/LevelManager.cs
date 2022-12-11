using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }
    public static event Action OnLevelComplete;

    [SerializeField] private GameObject levelLoadRoom;

    private static GameObject _tempLoadRoom;
    
    private static List<Scene> _levels;
    
    private int _levelID; // The current level's id

    private Transform _startPoint;
    private Transform _endPoint;

    public void Awake() {
        // If we don't currently know all levels, find them
        if (_levels == null) {
            _levels = new List<Scene>();
            _levels.Add(new Scene()); // Create a dummy scene so ID's line up with level numbers.
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

        _startPoint = transform.GetChild(0); // Get start door transform
        _endPoint = transform.GetChild(1); // Get end door transform
        
        // Get the ID of the current level
        _levelID = int.Parse(gameObject.scene.name.Split("vel")[1]);

        Debug.Log("LevelManager initialized for level " + _levelID);

        _tempLoadRoom = Instantiate(levelLoadRoom); // TODO: TEMP
        _tempLoadRoom.transform.position = new Vector3(0, -100, 0); // TODO: TEMP
        PlayerController.Instance.transform.SetParent(_tempLoadRoom.transform); // TODO: TEMP

        if (_tempLoadRoom) { // The player is loading into a new level. This is the new level
            SetupNewEntrance();
        }

        // unload async scene
        //StartCoroutine(UnloadAsyncScene(_levels[_levelIndex]));
    }

    private void SetupNewEntrance() {
        // Get the position of the entrance door so it can be used as an offset when teleporting the room to the entrance door.
        Vector3 doorPos = _tempLoadRoom.transform.GetChild(0).localPosition;
        _tempLoadRoom.transform.position = _startPoint.position - doorPos;
        
        _tempLoadRoom.transform.rotation = _startPoint.rotation;

        // open the door
    }

    private IEnumerator LoadAsyncScene(Scene scene) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
    
    private IEnumerator UnloadAsyncScene(Scene scene) {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        while (!asyncUnload.isDone) {
            yield return null;
        }
    }

    public void OnLevelEnd() {
        Debug.LogWarning("Number 1 victory royale");
    }
}
