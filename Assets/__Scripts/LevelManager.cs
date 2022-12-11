using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }
    public static event Action OnLevelLoad;
    public static event Action OnLevelComplete;

    public Vector3 PlayerSpawnPosition;

    [SerializeField] private GameObject levelLoadRoom;
    
    public void Awake() {
        if (Instance == null) Instance = this;
        else {
            Destroy(gameObject);
            Debug.LogWarning("LevelManager already exists. Destroying duplicate.");
        }
        OnLevelLoad?.Invoke();
    }

    public void OnDestroy() {
        
    }

    public void OnLevelEnd() {
        Debug.LogWarning("Number 1 victory royale");
    }
    
    
}
