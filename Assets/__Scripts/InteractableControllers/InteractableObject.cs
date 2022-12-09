using System.Linq;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public float interactionDistance = 3;
    
    protected PlayerController playerController;
    public virtual void PrimaryInteract() {throw new System.NotImplementedException();}
    public virtual void SecondaryInteract() {throw new System.NotImplementedException();}

    protected virtual void Awake() {
        playerController = FindObjectsOfType<PlayerController>().First();
    }
    protected virtual void Start() {}
    protected virtual void Update() {}
    protected virtual void FixedUpdate() {}

    public virtual bool CanInteract(Transform interactTransform) {
        return interactionDistance > 0 && ((transform.position - interactTransform.position).magnitude <= interactionDistance);
    }
}
