using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCube : InteractableObject {
    private Rigidbody rigidbody;

    private bool isHeld = false;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        canPush = true;
    }
    
    void Update() {
        if (isHeld) {
            rigidbody.position = Camera.main.transform.TransformPoint(Vector3.forward * 5);
        }
    }

    public override void Interact() {
        if (isHeld) {
            drop();
        }

        isHeld = !isHeld;
    }

    private void drop() {
        Debug.Log("drop the cube");
    }
}
