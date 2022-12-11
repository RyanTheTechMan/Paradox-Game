using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LevelLoadDoorObject : DoorObject {
    public void LockDoor() {
        // Create a box collider to block the player from entering the door
        SetActivation(false);
        BoxCollider _collider = GetComponent<BoxCollider>();
        _collider.enabled = true;

        enabled = false;
    }
}