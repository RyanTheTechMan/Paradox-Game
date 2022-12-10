using System.Collections;
using System.Linq;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public float interactionDistance = 3;
    
    [SerializeField] private bool createCounterpart;
    
    protected bool isFutureCounterpart;
    
    private int _leftEyeLayer;
    private Vector3 _lastLocation;
    protected InteractableObject Counterpart;
    
    protected PlayerController playerController;
    public virtual void PrimaryInteract() {throw new System.NotImplementedException();}
    public virtual void SecondaryInteract() {throw new System.NotImplementedException();}

    protected virtual void Awake() {
        playerController = PlayerController.Instance;
        _leftEyeLayer = LayerMask.NameToLayer("Left Eye");
    }

    protected virtual void Start() {
        if (createCounterpart) { // TODO: Maybe move to OnEnable to solve some random issues
            gameObject.SetActive(false);
            GameObject go = Instantiate(gameObject, transform.parent);
            gameObject.SetActive(true);
            Counterpart = go.GetComponent<InteractableObject>();
            Counterpart.createCounterpart = false;
            Counterpart.isFutureCounterpart = true;
            Counterpart.Counterpart = this;
            
            foreach (var child in Counterpart.transform.GetComponentsInChildren<Transform>(true)) {
                child.gameObject.layer = _leftEyeLayer;
            }
            
            StartCoroutine(SetActive()); // Waits a frame
        }
    }

    protected virtual void Update() {
        if (Counterpart) {
            if (!isFutureCounterpart)
                if (_lastLocation != transform.localPosition) CounterpartUpdate();
            _lastLocation = transform.localPosition;
        }
    }

    protected virtual void FixedUpdate() {}

    public virtual bool CanInteract(Transform interactTransform) {
        return interactionDistance > 0 && ((transform.position - interactTransform.position).magnitude <= interactionDistance);
    }

    protected virtual void CounterpartUpdate() {
        Counterpart.transform.localPosition = transform.localPosition;
        Counterpart.transform.localRotation = transform.localRotation;
    }

    private IEnumerator SetActive() {
        yield return 0;
        Counterpart.gameObject.SetActive(true);
    }
}
