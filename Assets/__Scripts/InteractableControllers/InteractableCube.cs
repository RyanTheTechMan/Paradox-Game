using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCube : InteractableObject {
    public float holdDistance = 1;
    public float cubeInteractionDistance = 3;

    private PlayerController _playerController;
    private Rigidbody rigidbody;

    private bool isHeld = false;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        
        canPush = true;
        interactionDistance = cubeInteractionDistance;
    }
    
    void FixedUpdate() {
        if (isHeld) {
            rigidbody.position = _playerController._camera.transform.TransformPoint(Vector3.forward * holdDistance);
        }
    }

    public override void Interact() {
        if (isHeld) {
            Drop();
        }
        else {
            PickUp();
        }

        isHeld = !isHeld;
    }

    public override bool CanInteract(Transform interactTransform) {
        return isHeld || base.CanInteract(interactTransform);
    }

    private void Drop() {
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.None;
    }

    private void PickUp() {
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
