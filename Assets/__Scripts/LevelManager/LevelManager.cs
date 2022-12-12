using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }
    public static event Action OnLevelComplete;
    public bool CanUsePortal = true;

    public GameObject levelLoadRoom;

    private static LevelLoadRoomHandler _tempLoadRoom;
    
    private static List<string> _levels;
    
    private int _levelID; // The current level's id

    private Transform _startPoint;
    private Transform _endPoint;

    private void Awake() {
        // If we don't currently know all levels, find them
        if (_levels == null) {
            _levels = new List<string>();
            _levels.Add(""); // Create a dummy scene so ID's line up with level numbers.
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                int lastSlash = path.LastIndexOf("/");
                string sceneName = path.Substring(lastSlash + 1, path.LastIndexOf(".") - lastSlash - 1);
                if (sceneName.StartsWith("level")) {
                    Debug.Log("Adding \"" + sceneName + "\" to level list.");
                    _levels.Add(sceneName);
                }
            }
        }
        
        // If LevelManager is not already set, set it. One per level.
        if (Instance) {
            Debug.LogWarning("LevelManager already exists. Disabling duplicate.");
            // Instance.gameObject.SetActive(false);
            Destroy(Instance.gameObject);
            
            // GameObject go = Instantiate(levelLoadRoom);
            // _tempLoadRoom = go.GetComponent<LevelLoadRoomHandler>();
        }
        
        Instance = this;

        _startPoint = transform.GetChild(0); // Get start door transform
        _endPoint = transform.GetChild(1); // Get end door transform

        string[] sceneNameSplit = gameObject.scene.name.Split("vel");
        if (sceneNameSplit.Length > 1) {
            int.TryParse(sceneNameSplit[1], out _levelID);
            Debug.Log("LevelManager initialized for level " + _levelID);
        }
        else {
            Debug.Log("Not a level scene. LevelManager not initialized. Loading level 1.");
            LoadNextLevel();
        }
    }

    private void Start() {
        if (_levelID != 0) { // If we are in a level. Set up the level load room(s)
            if (_startPoint.gameObject.activeInHierarchy) { // If there is no start door then this must be the first level
                SetupNewLevelEntrance();
            }
            SetupNewLevelExit();
        }
    }

    private void SetupNewLevelEntrance() {
        Debug.Log("Settings up new level entrance.");
        if (!_tempLoadRoom) { // We aren't coming from a loaded level. We have to create a room to load into.
            Debug.Log("Instantiate levelLoadRoom in entrance generation");
            GameObject go = Instantiate(levelLoadRoom);
            _tempLoadRoom = go.GetComponent<LevelLoadRoomHandler>();
        }
        
        PlayerController.Instance.gameObject.SetActive(false); // Disable player so it gets teleported with the room
        // Get the position of the entrance door so it can be used as an offset when teleporting the room to the entrance door location
        Vector3 doorPos = _tempLoadRoom.enterLevelDoor.transform.localPosition;
        _tempLoadRoom.transform.rotation = _startPoint.rotation * Quaternion.Euler(0, -90, 0);
        // Offset the position of the room so it is centered on the entrance door, using the rotation
        _tempLoadRoom.transform.position = _startPoint.position - _tempLoadRoom.transform.rotation * doorPos;
        PlayerController.Instance.gameObject.SetActive(true); // Re-enable player

        PlayerController.Instance.transform.SetParent(null);

        Debug.Log("Opening entrance door.");
        // open the door
        _tempLoadRoom.enterLevelDoor.SetActivation(true);
        Debug.Log("Done setting up new level entrance.");
    }

    private void SetupNewLevelExit() {
        Debug.Log("Settings up new level exit");
        GameObject go = Instantiate(levelLoadRoom);
        _tempLoadRoom = go.GetComponent<LevelLoadRoomHandler>();
        DontDestroyOnLoad(go); // We do this because this will be moved to the start of the next level.

        // Get the position of the exit door so it can be used as an offset when teleporting the room to the exit door location
        GameObject exitDoor = _tempLoadRoom.exitLevelDoor.gameObject;
        Vector3 doorPos = exitDoor.transform.localPosition;
        
        _tempLoadRoom.transform.rotation = _endPoint.rotation * Quaternion.Euler(0, -90, 0);
        // Offset the position of the room so it is centered on the exit door, using the rotation
        _tempLoadRoom.transform.position = _endPoint.position - _tempLoadRoom.transform.rotation * doorPos;

        Debug.Log("Done setting up new level exit");
    }

    public void LoadNextLevel() {
        if (SceneManager.GetActiveScene().name != "loader") {
            Debug.LogWarning("Level complete. Would load next level.");
            return;
        }
        StartCoroutine(LoadNextLevelAsync(_levelID));
    }
    
    public IEnumerator LoadNextLevelAsync(int currentLevel) {
        Debug.Log("Called async method");
        yield return null;
        Debug.Log("Starting load level");
        if (currentLevel < _levels.Count - 1) {
            Debug.Log("Loading New Level");
            if (_tempLoadRoom) {
                PlayerController.Instance.transform.SetParent(_tempLoadRoom.transform, true);
                PlayerController.Instance.gameObject.SetActive(false);
                _tempLoadRoom.transform.position = new Vector3(_tempLoadRoom.transform.position.x, _tempLoadRoom.transform.position.y - 100f, _tempLoadRoom.transform.position.z);
                PlayerController.Instance.gameObject.SetActive(true);
            }
            
            if (currentLevel == 0) {
                Debug.Log("No previous level to unload");
            }
            else {
                Debug.Log("Unloading previous level");
                // Don't destroy when we unload the level. If it does, then the code stops working.
                DontDestroyOnLoad(gameObject);
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_levels[currentLevel],
                    UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                while (!asyncUnload.isDone) {
                    yield return null;
                }
                Debug.Log("Done!");
            }

            Debug.Log("Ready, loading next level");
            yield return new WaitForSeconds(3);
            Debug.Log("Starting load rn");
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_levels[currentLevel + 1], LoadSceneMode.Additive);
            while (!asyncLoad.isDone) {
                yield return null;
            }
            
            Debug.Log("Done!");
            // Destroy(gameObject);
        }
        else {
            Debug.LogWarning("There are no more levels to load.");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelManager), true), CanEditMultipleObjects]
public class LevelManagerEditor : Editor {
    
    private Transform _startPoint;
    private Transform _endPoint;

    private static LevelLoadRoomHandler start;
    private static LevelLoadRoomHandler exit;
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        LevelManager lvlMgr = (LevelManager) target;

        if ((start || exit) && GUILayout.Button("Remove Previews")) {
            DestroyImmediate(start.gameObject);
            DestroyImmediate(exit.gameObject);
            start = null;
            exit = null;
        }
        else if (GUILayout.Button("Preview Level Load Rooms")) {
            _startPoint = lvlMgr.transform.GetChild(0); // Get start door transform
            _endPoint = lvlMgr.transform.GetChild(1); // Get end door transform
            
            GameObject go = Instantiate(lvlMgr.levelLoadRoom);
            exit = go.GetComponent<LevelLoadRoomHandler>();

            GameObject exitDoor = exit.transform.GetChild(0).GetChild(1).gameObject;
            Vector3 doorPos = exitDoor.transform.localPosition;
            exit.transform.rotation = _endPoint.rotation * Quaternion.Euler(0, -90, 0);
            exit.transform.position = _endPoint.position - exit.transform.rotation * doorPos;
            
            
            go = Instantiate(lvlMgr.levelLoadRoom);
            start = go.GetComponent<LevelLoadRoomHandler>();
            
            GameObject startDoor = exit.transform.GetChild(0).GetChild(0).gameObject;
            doorPos = startDoor.transform.localPosition;
            start.transform.rotation = _startPoint.rotation * Quaternion.Euler(0, -90, 0);
            start.transform.position = _startPoint.position - start.transform.rotation * doorPos;
            
            Undo.RegisterCreatedObjectUndo(start.gameObject, "Created Level Load Room");
            Undo.RegisterCreatedObjectUndo(exit.gameObject, "Created Level Load Room");
        }
    }
}
#endif