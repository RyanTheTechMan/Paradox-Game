using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class MovableObject : InteractableObject {
    protected Rigidbody _rigidbody;
    private const float _breakForce = 10000f;

    public bool canPush;
    public bool canHold;

    public bool isBeingHeld => PlayerController.Instance.holdingObject == this;
    
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
    
    public void Drop() {
        PlayerController.Instance.holdingObject = null;
        _rigidbody.useGravity = true;
        
        if (_holdPoint) DestroyImmediate(_holdPoint);
    }

    private void PickUp() {
        PlayerController.Instance.holdingObject = this;
        _rigidbody.useGravity = false;
        _rigidbody.transform.position = playerController.camera.transform.TransformPoint(Vector3.forward * 2);
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
        if (isBeingHeld) return true;
        if (PlayerController.Instance.holdingObject) return false;
        
        return base.CanInteract(interactTransform);
    }

    protected override void CounterpartUpdate() {
        base.CounterpartUpdate();
        MovableObject obj = (MovableObject)Counterpart; // Future object
        Rigidbody futureRigidbody = obj._rigidbody;
        if (futureRigidbody == null) return; // TODO: fixes errors. find another way to do this lmao
        futureRigidbody.velocity = _rigidbody.velocity;
        futureRigidbody.useGravity = _rigidbody.useGravity;
        futureRigidbody.isKinematic = _rigidbody.isKinematic;
        futureRigidbody.angularVelocity = _rigidbody.angularVelocity;
        
        Transform counterpartTransform = Counterpart.transform;
        if ((counterpartTransform.position - transform.position).magnitude > 1) {
            Instantiate(GameManager.Instance.destroyObjectParticles, counterpartTransform.position, transform.rotation);
            if (isFutureCounterpart) obj.Drop();
        }
        
        counterpartTransform.localPosition = transform.localPosition;
        counterpartTransform.localRotation = transform.localRotation;
    }
}