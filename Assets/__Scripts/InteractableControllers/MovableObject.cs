using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovableObject : InteractableObject {
    protected Rigidbody _rigidbody;
    private const float _breakForce = 10000f;

    public bool canPush;
    public bool canHold;

    public bool isBeingHeld { get; protected set; }

    private FixedJoint _holdPoint;
    
    public AudioClip[] pushSounds;
    public AudioClip[] dropSounds;

    protected override void Awake() {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected override void Update() {
        base.Update();
        if (_holdPoint == null && isBeingHeld) {
            Drop();
        }
    }
    
    public override void PrimaryInteract() {
        if (!canHold) return;
        if (isBeingHeld) Drop();
        else PickUp();
    }
    
    public override void SecondaryInteract() {
        if (!canHold) return;
        if (isBeingHeld) Throw();
        else PickUp();
    }
    
    private void Drop() {
        isBeingHeld = false;
        _rigidbody.useGravity = true;
        
        if (_holdPoint) DestroyImmediate(_holdPoint);
    }

    private void PickUp() {
        isBeingHeld = true;
        _rigidbody.useGravity = false;
        _rigidbody.transform.position = playerController._camera.transform.TransformPoint(Vector3.forward * 2);
        _holdPoint = playerController.hand.gameObject.AddComponent<FixedJoint>();
        _holdPoint.breakForce = _breakForce;
        _holdPoint.enableCollision = false;
        _holdPoint.connectedBody = _rigidbody;
    }
    
    private void Throw() {
        Drop();
        Vector3 direction = (transform.position - playerController.transform.position).normalized;
        _rigidbody.AddForce(direction * playerController.throwForce, ForceMode.Impulse);
    }
    
    public override bool CanInteract(Transform interactTransform) {
        return isBeingHeld || base.CanInteract(interactTransform);
    }
}