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

    protected override void Start() {
        base.Start();
        _leverHandle = transform.GetChild(0).gameObject;
        _indicatorLight = transform.GetChild(1).gameObject;
    }
    public override void PrimaryInteract() {
        if (isActive) Deactivate();
        else Activate();

        _indicatorLight.GetComponent<MeshRenderer>().material = isActive ? activatedMaterial : deactivatedMaterial;
        
        UpdateActivation();
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
        while (t < 1f) {
            t += Time.deltaTime * 5f;
            _leverHandle.transform.localRotation = Quaternion.Euler(Mathf.Lerp(isActive ? _defaultRot : _pressedRot, isActive ? _pressedRot : _defaultRot, t), 0, 0);
            yield return null;
        }
        
        _animating = false;
    }
    
    public override bool CanInteract(Transform interactTransform) {
        return !_animating && base.CanInteract(interactTransform);
    }
}
