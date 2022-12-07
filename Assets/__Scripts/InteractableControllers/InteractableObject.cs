using System;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public float interactionDistance = 3;
    
    protected PlayerController playerController;
    public virtual void PrimaryInteract() { }
    public virtual void SecondaryInteract() { }

    protected virtual void Awake() {
        playerController = FindObjectOfType<PlayerController>();
    }
    protected virtual void Start() {}
    protected virtual void Update() {}
    protected virtual void FixedUpdate() {}

    public virtual bool CanInteract(Transform interactTransform) {
        return interactionDistance > 0 && ((transform.position - interactTransform.position).magnitude <= interactionDistance);
    }
}
