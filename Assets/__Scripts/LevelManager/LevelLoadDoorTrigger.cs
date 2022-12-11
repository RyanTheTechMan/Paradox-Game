using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadDoorTrigger : MonoBehaviour
{
    public event Action Triggered;

    public bool isTriggered;
    private void OnTriggerEnter(Collider other) {
        if (isTriggered) return;
        isTriggered = true;
        Triggered?.Invoke();
    }
}
