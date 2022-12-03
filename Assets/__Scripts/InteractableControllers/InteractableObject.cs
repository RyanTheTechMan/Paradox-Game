using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableObject : MonoBehaviour {
    public float interactionDistance = 3;
    public bool canPush;

    public virtual void Interact() {
        
    }

    public virtual bool CanInteract(Transform interactTransform) {
        return (transform.position - interactTransform.position).magnitude <= interactionDistance;
    }
}
