using System.Linq;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public float interactionDistance = 3;
    
    [SerializeField] private bool createCounterpart;
    
    protected bool isFutureCounterpart;
    
    private int _leftEyeLayer;
    private Vector3 _lastLocation;
    protected InteractableObject _counterpart;
    
    protected PlayerController playerController;
    public virtual void PrimaryInteract() {throw new System.NotImplementedException();}
    public virtual void SecondaryInteract() {throw new System.NotImplementedException();}

    protected virtual void Awake() {
        playerController = PlayerController.Instance;
        _leftEyeLayer = LayerMask.NameToLayer("Left Eye");
    }

    protected virtual void Start() {
        if (createCounterpart) { // TODO: Maybe move to OnEnable to solve some random issues
            GameObject go = Instantiate(gameObject, transform.parent);
            _counterpart = go.GetComponent<InteractableObject>();
            _counterpart.createCounterpart = false;
            _counterpart.isFutureCounterpart = true;
            _counterpart._counterpart = this;
            
            foreach (var child in _counterpart.transform.GetComponentsInChildren<Transform>(true)) {
                child.gameObject.layer = _leftEyeLayer;
            }
        }
    }

    protected virtual void Update() {
        if (_counterpart) {
            if (isFutureCounterpart) UpdateFutureCounterpart();
            else if (_lastLocation != transform.localPosition) UpdatePresentCounterpart();

            _lastLocation = transform.localPosition;
        }
    }

    protected virtual void FixedUpdate() {}

    public virtual bool CanInteract(Transform interactTransform) {
        return interactionDistance > 0 && ((transform.position - interactTransform.position).magnitude <= interactionDistance);
    }
    
    protected virtual void UpdateFutureCounterpart() {}
    
    protected virtual void UpdatePresentCounterpart() {
        _counterpart.transform.localPosition = transform.localPosition;
        _counterpart.transform.localRotation = transform.localRotation;
    }
}
