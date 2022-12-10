using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Button : ActivatorObject
{
    // When object is on the button, it will be activated, only once all objects are off the button, it will be deactivated
    private readonly List<Collider> _objectsOnButton = new List<Collider>();

    private readonly float _defaultY = 0.15f;
    private readonly float _pressedY = 0.05f;
    
    private Transform _buttonTop;
    
    protected override void Awake()
    {
        base.Awake();
        _buttonTop = transform.GetChild(0);
    }
    
    private void OnTriggerEnter (Collider other) {
        if (!_objectsOnButton.Contains(other)) _objectsOnButton.Add(other);
        if (!IsActive) {
            Activate();
            UpdateActivation();
            Animate();
        }
    }
    private void OnTriggerExit (Collider other) {
        _objectsOnButton.Remove(other);
        if (_objectsOnButton.Count == 0) {
            Deactivate();
            UpdateActivation();
            Animate();
        }
    }

    private void Animate() {
        StartCoroutine(DoAnimate());
    }

    private IEnumerator DoAnimate() {
        const float speed = 1f;
        Vector3 pos = _buttonTop.localPosition;
        
        float startPos = pos.y;
        float endPos = IsActive ? _defaultY - _pressedY : _defaultY;
        float speedOffset = Mathf.Abs(startPos - endPos);
        speedOffset = speed * (1 / speedOffset);
        
        float t = 0f;
        bool animateState = IsActive;
        while (t < 1f && animateState == IsActive) {
            t += Time.deltaTime * speedOffset;
            pos.y = Mathf.Lerp(startPos, endPos, t);
            _buttonTop.localPosition = pos;
            yield return null;
        }
    }
    
}
