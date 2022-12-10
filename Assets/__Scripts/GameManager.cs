using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    public ParticleSystem destroyObjectParticles;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Debug.LogWarning("There can only be one GameManger in the scene.");
            Destroy(gameObject);
        }
    }
}
