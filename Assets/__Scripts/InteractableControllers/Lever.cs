using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ActivatorObject {
    public Material activatedMaterial;
    public Material deactivatedMaterial;
    
    private bool _animating;

    private readonly float _defaultRot = 25;
    private readonly float _pressedRot = -25;

    private GameObject _leverHandle;
    private GameObject _indicatorLight;

    protected override void Awake() {
        base.Awake();
        _leverHandle = transform.GetChild(0).gameObject;
        _indicatorLight = transform.GetChild(1).gameObject;
    }
    public override void PrimaryInteract() {
        if (IsActive) Deactivate();
        else Activate();

        Animate();
    }
    
    private void Animate() {
        if (!_animating) {
            _animating = true;
            StartCoroutine(DoAnimate());
        }
    }
    
    private IEnumerator DoAnimate() {
        var t = 0f;
        bool updatedActivation = false;
        while (t < 1f) {
            t += Time.deltaTime * 5f;
            _leverHandle.transform.localRotation = Quaternion.Euler(Mathf.Lerp(IsActive ? _defaultRot : _pressedRot, IsActive ? _pressedRot : _defaultRot, t), 0, 0);
            if (!updatedActivation && t > 0.5f) {
                UpdateActivation();
                _indicatorLight.GetComponent<MeshRenderer>().material = IsActive ? activatedMaterial : deactivatedMaterial;
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
