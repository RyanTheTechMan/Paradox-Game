using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }
    public static event Action OnLevelComplete;
    public bool CanUsePortal = true;
    [SerializeField] private bool createCounterpart;

    public GameObject levelLoadRoom;

    [NonSerialized] public static LevelLoadRoomHandler tempLoadRoom;
    
    private static List<string> _levels;
    
    private int _levelID; // The current level's id

    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    
    [NonSerialized] public static int forceLevel = -1; // Used to force a level to load. -1 means no level is forced.
    private bool unloadPreviousLevel = true; // Used to unload the previous level.

    private const bool debug = false;

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
                    _levels.Add(sceneName);
                }
            }
        }
        
        // If LevelManager is not already set, set it. One per level.
        if (Instance) {
            if (debug) Debug.LogWarning("LevelManager already exists. Disabling duplicate.");
            Destroy(Instance.gameObject);
        }
        
        Instance = this;

        string[] sceneNameSplit = gameObject.scene.name.Split("vel");
        if (sceneNameSplit.Length > 1) {
            int.TryParse(sceneNameSplit[1], out _levelID);
        }
        else {
            if (debug) Debug.Log("Loading next level from awake.");
            LoadNextLevel();
        }
    }

    private void Start() {
        if (_levelID == 0) return;
        // We are in a level. Set up the level load room(s)
        if (_startPoint) SetupNewLevelEntrance();
        if (_endPoint) SetupNewLevelExit();
    }

    private void SetupNewLevelEntrance() {
        if (!tempLoadRoom) { // We aren't coming from a loaded level. We have to create a room to load into.
            if (debug) Debug.Log("There is no entrance temp load room. Creating one.");
            GameObject go = Instantiate(levelLoadRoom);
            tempLoadRoom = go.GetComponent<LevelLoadRoomHandler>();
        }
        else {
            if (debug) Debug.Log("We are coming from a loaded level. No need to create a new Entrance room.");
            PlayerController.Instance.transform.SetParent(tempLoadRoom.transform);
            
        }
        PlayerController.Instance.gameObject.SetActive(false); // Disable player so it gets teleported with the room

        // Get the position of the entrance door so it can be used as an offset when teleporting the room to the entrance door location
        Vector3 doorPos = tempLoadRoom.enterLevelDoor.transform.localPosition;
        doorPos.y = 0;
        doorPos.z = 0;
        
        if (debug) Debug.Log("Entrance wanted at: " + _startPoint.position);
        
        tempLoadRoom.transform.rotation = _startPoint.rotation * Quaternion.Euler(0, -90, 0);
        // Offset the position of the room so it is centered on the entrance door, using the rotation
        tempLoadRoom.transform.position = _startPoint.position - tempLoadRoom.transform.rotation * doorPos;
        
        tempLoadRoom.gameObject.transform.SetParent(transform);

        if (debug) Debug.Log("Entrance set to: " + tempLoadRoom.transform.position);

        PlayerController.Instance.gameObject.SetActive(true); // Re-enable player

        PlayerController.Instance.transform.SetParent(null);
        DontDestroyOnLoad(PlayerController.Instance);

        // open the door
        tempLoadRoom.enterLevelDoor.SetActivation(true);
    }

    private void SetupNewLevelExit() {
        if (debug) Debug.Log("There is no exit temp load room. Creating one.");
        GameObject go = Instantiate(levelLoadRoom);
        tempLoadRoom = go.GetComponent<LevelLoadRoomHandler>();
        
        DontDestroyOnLoad(go); // We do this because this will be moved to the start of the next level.

        if (createCounterpart) {
            tempLoadRoom.exitLevelDoor.createCounterpart = true;

            foreach (Transform child in tempLoadRoom.exitLevelDoor.transform) {
                child.gameObject.layer = PlayerController.Instance.handheldPortal.layerRightEye;
            }
        }

        // tempLoadRoom.exitLevelDoor.counterpartParent = tempLoadRoom.enterLevelDoor.transform;

        // Get the position of the exit door so it can be used as an offset when teleporting the room to the exit door location
        GameObject exitDoor = tempLoadRoom.exitLevelDoor.gameObject;
        Vector3 doorPos = exitDoor.transform.position;
        doorPos.y = 0;
        doorPos.z = 0;

        if (debug) Debug.Log("Exit wanted at: " + _endPoint.position);
        
        tempLoadRoom.transform.rotation = _endPoint.rotation * Quaternion.Euler(0, -90, 0);
        // Offset the position of the room so it is centered on the exit door, using the rotation
        tempLoadRoom.transform.position = _endPoint.position - tempLoadRoom.transform.rotation * doorPos;
        
        
        if (debug) Debug.Log("Set exit to: " + tempLoadRoom.transform.position);
    }

    public void LoadNextLevel() {
        if (SceneManager.GetActiveScene().name != "loader") {
            if (debug) Debug.Log("Level complete. Attempting to load next level.");
            forceLevel = _levelID + 1;
            SceneManager.LoadScene("loader");
            return;
        }
        
        if (forceLevel != -1) {
            _levelID = forceLevel;
            forceLevel = -1;
            unloadPreviousLevel = false;
        }
        else {
            _levelID++;
            unloadPreviousLevel = true;
        }

        StartCoroutine(LoadNextLevelAsync(_levelID));
    }
    
    public IEnumerator LoadNextLevelAsync(int nextLevelID) {
        yield return null;
        if (nextLevelID < _levels.Count) {
            if (tempLoadRoom) {
                PlayerController.Instance.transform.SetParent(tempLoadRoom.transform, true);
                PlayerController.Instance.gameObject.SetActive(false);
                tempLoadRoom.transform.position = new Vector3(tempLoadRoom.transform.position.x, tempLoadRoom.transform.position.y - 100f, tempLoadRoom.transform.position.z);
                PlayerController.Instance.gameObject.SetActive(true);
            }

            if (unloadPreviousLevel && nextLevelID != 1  && forceLevel == -1) {
                // Don't destroy when we unload the level. If it does, then the code stops working.
                DontDestroyOnLoad(gameObject);
                
                if (debug) Debug.Log("unloading " + (nextLevelID - 1));
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_levels[nextLevelID-1],
                    UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                while (!asyncUnload.isDone) {
                    yield return null;
                }
            }

            // yield return new WaitForSeconds(3);
            if (debug) Debug.Log("loading " + nextLevelID);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_levels[nextLevelID], LoadSceneMode.Additive);
            while (!asyncLoad.isDone) {
                yield return null;
            }
        }
        else {
            Debug.LogWarning("There are no more levels to load.");
            DestroyImmediate(PlayerController.Instance.gameObject);
            DestroyImmediate(_tempLoadRoom.gameObject);
            DestroyImmediate(GameManager.Instance.gameObject);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
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
            Vector3 doorPos = exitDoor.transform.position;
            exit.transform.rotation = _endPoint.rotation * Quaternion.Euler(0, -90, 0);
            exit.transform.position = _endPoint.position - exit.transform.rotation * doorPos;
            
            
            go = Instantiate(lvlMgr.levelLoadRoom);
            start = go.GetComponent<LevelLoadRoomHandler>();
            
            GameObject startDoor = exit.transform.GetChild(0).GetChild(0).gameObject;
            doorPos = startDoor.transform.position;
            start.transform.rotation = _startPoint.rotation * Quaternion.Euler(0, -90, 0);
            start.transform.position = _startPoint.position - start.transform.rotation * doorPos;
            
            Undo.RegisterCreatedObjectUndo(start.gameObject, "Created Level Load Room");
            Undo.RegisterCreatedObjectUndo(exit.gameObject, "Created Level Load Room");
        }
    }
}
#endif