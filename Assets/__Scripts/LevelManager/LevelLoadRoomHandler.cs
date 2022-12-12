using System;
using System.Collections;
using UnityEngine;

public class LevelLoadRoomHandler : MonoBehaviour {
    [NonSerialized] public LevelLoadDoorObject enterLevelDoor;
    [NonSerialized] public LevelLoadDoorObject exitLevelDoor;
    
    [NonSerialized] public LevelLoadDoorTrigger enterLevelDoorTrigger;
    [NonSerialized] public LevelLoadDoorTrigger exitLevelDoorTrigger;
    
    private void Awake() {
        enterLevelDoor = transform.GetChild(0).GetChild(0).GetComponent<LevelLoadDoorObject>();
        exitLevelDoor = transform.GetChild(0).GetChild(1).GetComponent<LevelLoadDoorObject>();
        
        enterLevelDoorTrigger = transform.GetChild(1).GetChild(0).GetComponent<LevelLoadDoorTrigger>();
        exitLevelDoorTrigger = transform.GetChild(1).GetChild(1).GetComponent<LevelLoadDoorTrigger>();
        
        enterLevelDoorTrigger.Triggered += EnterLevelDoorTriggered;
        exitLevelDoorTrigger.Triggered += ExitLevelDoorTriggered;
    }

    private void EnterLevelDoorTriggered() { // Player walked outside of LevelLoadRoom
        enterLevelDoor.LockDoor();
    }
    
    private void ExitLevelDoorTriggered() { // Player walked inside of LevelLoadRoom
        exitLevelDoor.LockDoor();
        StartCoroutine(LoadNextLevelAnimation());
    }

    private IEnumerator LoadNextLevelAnimation() {
        Debug.Log("Playing animation... (Wait 3 seconds)");
        // yield return new WaitForSeconds(3f);
        yield return null;
        Debug.Log("Calling LoadNextLevel()...");
        LevelManager.Instance.LoadNextLevel();
    }
}
