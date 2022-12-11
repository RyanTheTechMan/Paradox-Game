using System.Collections;
using UnityEngine;

public class DoorObject : ActivatableObject {
    private Transform _leftDoor;
    private Transform _rightDoor;
    
    private float _restingPos;
    private float _moveDistance;
    
    protected new BoxCollider collider;
    
    protected override void Awake() {
        base.Awake();
        _leftDoor = transform.GetChild(0).gameObject.transform;
        _rightDoor = transform.GetChild(1).gameObject.transform;
        _restingPos = _leftDoor.transform.localPosition.x;
        _moveDistance = _leftDoor.GetComponent<BoxCollider>().size.x * 0.95f;
    }
    
    protected override void OnActiveChange() { 
        StartCoroutine(DoAnimate());
    }

    private IEnumerator DoAnimate() {
        const float speed = 2f;
        Vector3 pos = _leftDoor.localPosition;
        
        float startPos = pos.x;
        float endPos = IsActive ? Mathf.Abs(_restingPos) + _moveDistance : _restingPos;
        float speedOffset = Mathf.Abs(startPos - endPos);
        speedOffset = speed * (1 / speedOffset);
        
        float t = 0f;
        bool animateState = IsActive;
        while (t < 1f && animateState == IsActive) {
            t += Time.deltaTime * speedOffset;
            pos.x = Mathf.Lerp(startPos, endPos, t);
            _leftDoor.localPosition = pos;
            _rightDoor.localPosition = -pos;
            yield return null;
        }
    }
    
    protected override void CounterpartUpdate() {
        base.CounterpartUpdate();
        DoorObject obj = (DoorObject)Counterpart; // Future object
        if (obj.IsActive != IsActive) {
            obj.OnActiveChange();
            obj.PlaySound();
        }
    }
}