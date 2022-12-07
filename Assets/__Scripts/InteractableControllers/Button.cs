using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : ActivatorObject
{
    private void OnTriggerEnter(Collider other) {
        Activate();
    }
    
    private void OnTriggerExit(Collider other) {
        Deactivate();
    }
}
