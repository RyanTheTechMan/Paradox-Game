using UnityEngine;

public class DoorObject : ActivatableObject {
    protected new void Activate() {
        Debug.Log("Door opened");
    }
    
    protected new void Deactivate() {
        Debug.Log("Door ID " + id + " closed");
    }
}