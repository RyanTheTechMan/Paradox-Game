using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class CursorController : MonoBehaviour {
    public static CursorController instance;
    public static InteractableObject selected;
    
    public Image cursor;
    
    private Camera _camera;
    private PlayerController _playerController;

    private Color _highlightColor = new Color32(255, 255, 255, 200);
    private Color _normalColor = new Color32(36, 36, 36, 100);
    
    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Debug.LogWarning("There can only be one CursorController in the scene.");
            Destroy(gameObject);
        }
        
        _playerController = gameObject.GetComponentInParent<PlayerController>();
        _camera = _playerController._camera;
    }

    void Update() {
        DoRaycast();
    }

    private void DoRaycast() {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit)) {
            InteractableObject interactable = hit.transform.gameObject.GetComponent<InteractableObject>();
            if (interactable && interactable.CanInteract(_playerController.transform)) {
                cursor.color = _highlightColor;
                selected = interactable;
                return;
            }
        }

        selected = null;
        cursor.color = _normalColor;
    }
}
