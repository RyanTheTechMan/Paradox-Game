using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public void OnLevelEnd() {
        Debug.LogWarning("Number 1 victory royale");
    }

    public void Awake() {
        LevelGoalHandler.OnLevelComplete += OnLevelEnd;
    }
}
