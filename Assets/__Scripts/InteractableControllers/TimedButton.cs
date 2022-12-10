using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimedButton : ActivatorObject {
    public Material activatedMaterial;
    public Material deactivatedMaterial;

    private bool _animating;
    
    [Range(1, 60)] public int activationTime;
    private int currentTime;

    private readonly float _defaultX = 0.08f;
    private readonly float _pressedX = 0.05f;
    private readonly String _placeHolderText = "--";
    
    private Transform _button;
    [SerializeField] private TextMeshProUGUI _timer;
    
    protected override void Awake() {
        base.Awake();
        _button = transform.GetChild(0);

        _timer.text = _placeHolderText;
    }

    public override void PrimaryInteract() {
        if (IsActive) Deactivate();
        else Activate();
    }

    protected override void Activate() {
        base.Activate();
        currentTime = activationTime + 1;
        InvokeRepeating("Countdown", 0, 1.0f); 
        
        Animate();
    }

    protected override void Deactivate() {
        base.Deactivate();
        _timer.text = _placeHolderText;
        CancelInvoke("Countdown");
        
        Animate();
    }
    
    public override void SecondaryInteract() { }

    private void Animate() {
        if (!_animating) {
            _animating = true;
            StartCoroutine(DoAnimate());
        }
    }

    private void Countdown() {
        if (--currentTime < 0) {
            Deactivate();
            return;
        }

        PlaySound();
        _timer.text = currentTime.ToString();
    }

    private IEnumerator DoAnimate() {
        const float speed = .25f;
        Vector3 pos = _button.localPosition;
        
        float startPos = pos.x;
        float endPos = IsActive ? _pressedX : _defaultX;
        float speedOffset = Mathf.Abs(startPos - endPos);
        speedOffset = speed * (1 / speedOffset);
        
        float t = 0f;
        bool updatedActivation = false;
        while (t < 1f) {
            t += Time.deltaTime * speedOffset;
            pos.x = Mathf.Lerp(startPos, endPos, t);
            _button.localPosition = pos;
            
            if (!updatedActivation && t > 0.5f) {
                UpdateActivation();
                _button.GetComponent<MeshRenderer>().material = IsActive ? activatedMaterial : deactivatedMaterial;
                updatedActivation = true;
            }
            
            yield return null;
        }
        
        _animating = false;
    }
    
    public override bool CanInteract(Transform interactTransform) {
        return !_animating && base.CanInteract(interactTransform);
    }
}
