using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ActivatableObject : InteractableObject {
    [SerializeField, Header("Trigger / Activatable when opposite is true")]
    protected bool outputInverted;
    
    [SerializeField, Tooltip("When all activator objects ID are activated.")]
    private uint _id;
    public uint ID => _id; // Prevents changing ID in code

    public bool IsActive { get; private set; }

    private bool _lastActiveState;
    
    protected List<ActivatableObject> LinkedObjects; // All objects with the same ID as this object.

    public List<ActivatableObject> GetActivatable { // Objects that can be activated by this ID.
        get {
            if (LinkedObjects == null) ConfigureLinkedObjects();
            return LinkedObjects.Where(x => x is not ActivatorObject).ToList();
        }
    }

    public List<ActivatorObject> GetActivators { // Objects that need to be activated to activate this ID.
        get {
            if (LinkedObjects == null) ConfigureLinkedObjects();
            return LinkedObjects.OfType<ActivatorObject>().ToList();
        }
    }
    
    public void ConfigureLinkedObjects() {
        // Get the linked object of this ID. If none, create a new list.
        LinkedObjects = FindObjectsOfType<ActivatableObject>().FirstOrDefault(x => x.ID == ID && x != this)?.LinkedObjects ?? new List<ActivatableObject>();
        LinkedObjects.Add(this);
    }
    
    protected override void Awake() {
        base.Awake();
        ConfigureLinkedObjects(); // TODO: May not be needed.
    }

    protected void UpdateActivation() { // Must be called when the state of the activator objects change. (Separate function to account for animations)
        // for all Activators, if they are all active, set the isActive value and call the OnActiveChange() method on objects that are Activatable.
        bool allActive = GetActivators.TrueForAll(x => x.IsActive);
        if (allActive != _lastActiveState) {
            GetActivatable.ForEach(x => {
                x.IsActive = x.outputInverted ? !allActive : allActive;
                x.OnActiveChange();
            });
            _lastActiveState = allActive;
        }
    }
    
    protected virtual void Activate() {
        IsActive = !outputInverted;
    }
    
    protected virtual void Deactivate() {
        IsActive = outputInverted;
    }

    protected virtual void OnActiveChange() {throw new NotImplementedException();}
}

#if UNITY_EDITOR
[CustomEditor(typeof(ActivatableObject), true), CanEditMultipleObjects]
public class ActivatableObjectEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        ActivatableObject ao = (ActivatableObject) target;
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.LabelField("Activators:");
        foreach (ActivatableObject activatable in FindObjectsOfType<ActivatableObject>().OfType<ActivatorObject>().Where(x => x.ID == ao.ID).Reverse()) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(activatable.IsActive ? "✓" : "✖", GUILayout.Width(20));
            EditorGUILayout.ObjectField(activatable.gameObject, typeof(GameObject), true);
            GUILayout.EndHorizontal();
        }
        
        EditorGUILayout.LabelField("Activatables:");
        foreach (ActivatableObject activatable in FindObjectsOfType<ActivatableObject>().Where(x => x is not ActivatorObject).Reverse()) {
            if (activatable.ID == ao.ID) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(activatable.IsActive ? "✓" : "✖", GUILayout.Width(20));
                EditorGUILayout.ObjectField(activatable.gameObject, typeof(GameObject), true);
                GUILayout.EndHorizontal();
            }
        }
        EditorGUI.EndDisabledGroup();
    }
}
#endif