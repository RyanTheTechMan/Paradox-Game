using System.Collections;
using UnityEngine;

public class DoorObject : ActivatableObject {
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
    
    protected override void OnActiveChange() {
        StartCoroutine(DoAnimate());
    }
    
    private IEnumerator DoAnimate() {
        float startPos = _leftDoor.transform.localPosition.x;
        float endPos = IsActive ? Mathf.Abs(_restingPos) + _moveDistance : _restingPos;
        float speed = Mathf.Abs(startPos - endPos);
        
        var t = 0f;
        bool animateState = IsActive;
        while (t < 1f && animateState == IsActive) {
            t += Time.deltaTime * 2f * (1 / speed);
            _leftDoor.transform.localPosition = new Vector3(Mathf.Lerp(startPos, endPos, t), 0, -0.5f);
            _rightDoor.transform.localPosition = new Vector3(Mathf.Lerp(-startPos, -endPos, t), 0, 0.5f);
            yield return null;
        }
    }
}