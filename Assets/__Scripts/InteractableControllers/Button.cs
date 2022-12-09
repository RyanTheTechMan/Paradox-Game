using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : ActivatorObject
{
    // When object is on the button, it will be activated, only once all objects are off the button, it will be deactivated
    private readonly List<Collider> _objectsOnButton = new List<Collider>();

    private readonly float _defaultY = 0.15f;
    private readonly float _pressedY = 0.05f;
    
    private bool _animating;
    private GameObject _buttonTop;
    
    protected override void Awake()
    {
        base.Awake();
        _buttonTop = transform.GetChild(0).gameObject;
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
        if (!_animating) {
            _animating = true;
            StartCoroutine(DoAnimate());
        }
    }

    private IEnumerator DoAnimate() {
        var t = 0f;
        while (t < 1f) {
            t += Time.deltaTime * 5f;
            _buttonTop.transform.localPosition = new Vector3(0, Mathf.Lerp(IsActive ? _defaultY : _pressedY, IsActive ? _pressedY : _defaultY, t), 0);
            yield return null;
        }
        
        _animating = false;
    }
    
}
