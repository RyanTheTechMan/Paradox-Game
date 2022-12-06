using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovableObject : InteractableObject {
    
    protected Rigidbody _rigidbody;
    protected float breakForce = 10000f;

    public bool canPush;
    public bool canHold;
    
    [NonSerialized]
    public bool beingHeld;
    
    private FixedJoint _holdPoint;

    private protected new void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private protected new void Update() {
        if (_holdPoint == null && beingHeld) {
            Drop();
        }
    }
    
    public override void PrimaryInteract() {
        base.PrimaryInteract();
        if (!canHold) return;

        if (beingHeld) Drop();
        else PickUp();
    }
    
    public override void SecondaryInteract() {
        base.SecondaryInteract();
        if (!canHold) return;

        if (beingHeld) Throw();
        else PickUp();
    }
    
    private void Drop() {
        beingHeld = false;
        _rigidbody.useGravity = true;
        
        if (_holdPoint) DestroyImmediate(_holdPoint);
    }

    private void PickUp() {
        beingHeld = true;
        _rigidbody.useGravity = false;

        _rigidbody.transform.position = playerController._camera.transform.TransformPoint(Vector3.forward * 2);
        _holdPoint = playerController.hand.gameObject.AddComponent<FixedJoint>();
        _holdPoint.breakForce = breakForce;
        _holdPoint.enableCollision = false;
        _holdPoint.connectedBody = _rigidbody;
    }
    
    private void Throw() {
        if (!beingHeld) return;
        Drop();
        Vector3 direction = (transform.position - playerController.transform.position).normalized;
        _rigidbody.AddForce(direction * playerController.throwForce, ForceMode.Impulse);
    }
    
    public override bool CanInteract(Transform interactTransform) {
        return beingHeld || base.CanInteract(interactTransform);
    }
}