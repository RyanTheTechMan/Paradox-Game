using System.Collections;
using UnityEngine;

public class DoorObject : ActivatableObject {
    private bool lastState;
    private bool _animating;

    private GameObject _leftDoor;
    private GameObject _rightDoor;
    private float _restingPos;
    private float _moveDistance;
    
    protected override void Awake() {
        base.Awake();
        _leftDoor = transform.GetChild(0).gameObject;
        _rightDoor = transform.GetChild(1).gameObject;
        _restingPos = _leftDoor.transform.localPosition.x;
        _moveDistance = _leftDoor.GetComponent<BoxCollider>().size.x * 0.95f;
    }
    
    protected override void OnActiveChange(bool activate) {
        if (lastState == activate) Debug.LogWarning("Caught duplicate state change. This is a bug.");
        else {
            if (!_animating) {
                _animating = true;
                StartCoroutine(DoAnimate());
            }
            lastState = activate;
        }
    }
    
    private IEnumerator DoAnimate() {
        float startPos = _leftDoor.transform.localPosition.x;
        float endPos = isActive ? Mathf.Abs(_restingPos) + _moveDistance : _restingPos;
        float speed = Mathf.Abs(startPos - endPos);
        
        var t = 0f;
        float startPos = _leftDoor.transform.localPosition.x;
        float endPos = isActive ? Mathf.Abs(_restingPos) + _moveDistance : _restingPos;
        while (t < 1f) {
            t += Time.deltaTime * 2f * (1 / speed);
            var pos = Mathf.Lerp(startPos, endPos, t);
            _leftDoor.transform.localPosition = new Vector3(pos, _leftDoor.transform.localPosition.y, _leftDoor.transform.localPosition.z);
            _rightDoor.transform.localPosition = new Vector3(-pos, _rightDoor.transform.localPosition.y, _rightDoor.transform.localPosition.z);
            yield return null;
        }
        _animating = false;
    }
}