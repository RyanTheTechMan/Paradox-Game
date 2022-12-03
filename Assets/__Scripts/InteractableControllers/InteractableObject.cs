using System;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public float interactionDistance = 3;
    
    protected PlayerController playerController;
    
    public virtual void Interact() {}
    
    private protected void Awake() {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }
    
    private protected void Start() {
        
    }

    public virtual bool CanInteract(Transform interactTransform) {
        return (transform.position - interactTransform.position).magnitude <= interactionDistance;
    }

    
}
