using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCube : MovableObject {
    public float holdDistance = 1;
    public float cubeInteractionDistance = 3;

    private protected new void Awake() {
        canPush = true;
        canHold = true;
        
        _rigidbody = GetComponent<Rigidbody>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        
        interactionDistance = cubeInteractionDistance;
    }

}
