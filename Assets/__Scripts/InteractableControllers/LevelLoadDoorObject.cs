using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LevelLoadDoorObject : DoorObject {
    public Transform counterpartParent;

    // protected override void Start() {
    //     base.Start();
    //     gameObject.transform.SetParent(counterpartParent);
    // }

    public void LockDoor() {
        // Create a box collider to block the player from entering the door
        SetActivation(false);
        BoxCollider _collider = GetComponent<BoxCollider>();
        _collider.enabled = true;

        enabled = false;
    }
}