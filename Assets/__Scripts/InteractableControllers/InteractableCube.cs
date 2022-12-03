using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCube : InteractableObject {
    private Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    void Update() {
        
    }

    public override void Interact() {
        Debug.Log("INTERACT WITH CUBE");
    }
}
