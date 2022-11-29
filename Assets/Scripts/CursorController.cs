using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// There is a Unity bug where it does not lock until you click lol
public class CursorController : MonoBehaviour {
    public bool lockedByDefault = true;
    private bool isLocked;
    
    void Start() {
        isLocked = lockedByDefault;
        if (lockedByDefault) {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
            isLocked = !isLocked;
        }
    }
}
