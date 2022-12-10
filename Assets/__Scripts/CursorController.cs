using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class CursorController : MonoBehaviour {
    public static CursorController Instance;
    public static InteractableObject Selected;
    
    [SerializeField] private Image cursor;
    [SerializeField] private LayerMask handheldPortalUp;
    [SerializeField] private LayerMask handheldPortalDown;
    
    private Camera _camera;
    private PlayerController _playerController;
    private HandheldPortal _handheldPortal;

    private readonly Color _highlightColor = new Color32(255, 255, 255, 200);
    private readonly Color _normalColor = new Color32(36, 36, 36, 100);
    private const float MaxRayDistance = 100f; // Max distance of raycast | Default: Mathf.Infinity


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Debug.LogWarning("There can only be one CursorController in the scene. Destroying this one.");
            Destroy(gameObject);
        }
        
        _playerController = PlayerController.Instance;
        _camera = _playerController._camera;
        
        _handheldPortal = PlayerController.Instance._handheldPortal;
    }

    private void Update() {
        DoRaycast();
    }

    private void DoRaycast() {
        if (Physics.Raycast(_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out RaycastHit hit, MaxRayDistance, _handheldPortal.isPortalActive ? handheldPortalUp : handheldPortalDown)) {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable && interactable.CanInteract(_playerController.transform)) {
                cursor.color = _highlightColor;
                Selected = interactable;
                return;
            }
        }
        Selected = null;
        cursor.color = _normalColor;
    }
}
