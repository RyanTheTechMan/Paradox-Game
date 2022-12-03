using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableObject : MonoBehaviour {
    public bool canPush;

    public virtual void Interact() {
        
    }
}
