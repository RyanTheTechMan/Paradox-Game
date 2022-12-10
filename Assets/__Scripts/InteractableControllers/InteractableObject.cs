using System.Linq;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public float interactionDistance = 3;
    public bool hasCounterpart;
    
    private int _leftEyeLayer;
    private Vector3 _lastLocation;
    private GameObject _counterpart;
    
    protected PlayerController playerController;
    public virtual void PrimaryInteract() {throw new System.NotImplementedException();}
    public virtual void SecondaryInteract() {throw new System.NotImplementedException();}

    protected virtual void Awake() {
        playerController = FindObjectsOfType<PlayerController>().First();
        _leftEyeLayer = LayerMask.NameToLayer("Left Eye");
    }

    protected virtual void Start() {
        if (hasCounterpart) {
            _counterpart = Instantiate(this.gameObject, transform.parent);
            _counterpart.GetComponent<InteractableObject>().hasCounterpart = false;

            foreach (var child in _counterpart.transform.GetComponentsInChildren<Transform>(true)) {
                child.gameObject.layer = _leftEyeLayer;
            }
        }
    }

    protected virtual void Update() {
        if (_counterpart) {
            if (_lastLocation != transform.localPosition) {
                _counterpart.transform.localPosition = transform.localPosition;
                _counterpart.transform.localRotation = transform.localRotation;
            }

            _lastLocation = transform.localPosition;
        }
    }
    protected virtual void FixedUpdate() {}

    public virtual bool CanInteract(Transform interactTransform) {
        return interactionDistance > 0 && ((transform.position - interactTransform.position).magnitude <= interactionDistance);
    }
}
