using UnityEngine;

public class DoorObject : ActivatableObject {
    private bool lastState;
    protected override void OnActiveChange(bool activate) {
        if (lastState == activate) Debug.LogWarning("Caught duplicate state change. This is a bug.");
        else if (activate) {
            transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            Debug.Log("Door " + id + " opened");
            lastState = true;
        } else {
            transform.position = new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);
            Debug.Log("Door " + id + " closed");
            lastState = false;
        }
    }
}