using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Button : ActivatorObject {
    // When object is on the button, it will be activated, only once all objects are off the button, it will be deactivated
    private readonly List<Collider> _objectsOnButton = new List<Collider>();

    private readonly float _defaultY = 0.15f;
    private readonly float _pressedY = 0.05f;
    
    private Transform _buttonTop;
    
    protected override void Awake() {
        base.Awake();
        _buttonTop = transform.GetChild(0);
    }
    
    private void OnTriggerEnter (Collider other) {
        if (!CanCollide(other.gameObject)) return;
        if (!_objectsOnButton.Contains(other)) _objectsOnButton.Add(other);
        if (!IsActive) {
            Activate();
            UpdateActivation();
            Animate();
        }
    }

    private void OnTriggerExit (Collider other) {
        if (!CanCollide(other.gameObject)) return;
        _objectsOnButton.Remove(other);
        if (_objectsOnButton.Count == 0) {
            Deactivate();
            UpdateActivation();
            Animate();
        }
    }
    
    private bool CanCollide(GameObject other) {
        if (other.layer == gameObject.layer) return true;
        return (other.layer == playerController.playerLayer && other.layer != playerController._handheldPortal.nonInteractableLayer);
    }

    private void Animate() {
        StartCoroutine(DoAnimate());
    }

    private IEnumerator DoAnimate() {
        const float speed = 0.1f;
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
    
    protected override void CounterpartUpdate() {
        base.CounterpartUpdate();
        Button obj = (Button)Counterpart; // Future object
        if (obj.IsActive != IsActive) {
            if (IsActive) obj.Activate();
            else obj.Deactivate();
            obj.UpdateActivation();
            obj.Animate();
        }
    }
}
