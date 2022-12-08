using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class CursorController : MonoBehaviour {
    public static CursorController instance;
    public static InteractableObject selected;
    
    public Image cursor;
    
    private Camera _camera;
    private PlayerController _playerController;

    private readonly Color _highlightColor = new Color32(255, 255, 255, 200);
    private readonly Color _normalColor = new Color32(36, 36, 36, 100);

    public LayerMask handheldPortalUp;
    public LayerMask handheldPortalDown;
    
    private HandheldPortal _handheldPortal;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Debug.LogWarning("There can only be one CursorController in the scene.");
            Destroy(gameObject);
        }
        
        _playerController = gameObject.GetComponentInParent<PlayerController>();
        _camera = _playerController._camera;
        
        _handheldPortal = FindObjectsOfType<PlayerController>().First().GetComponentInChildren<HandheldPortal>();
    }

    private void Update() {
        DoRaycast();
    }

    private void DoRaycast() {
        if (Physics.Raycast(_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out RaycastHit hit, Mathf.Infinity, _handheldPortal.isPortalActive ? handheldPortalUp : handheldPortalDown)) {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
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
