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
    public uint id {
        get => _id;
        protected set => _id = value;
    }

    private bool _isActive;
    public bool isActive {
        get => _isActive;
        protected set => _isActive = value;
    }
    
    private bool _lastActiveState;
    
    protected List<ActivatableObject> linkedObjects; // All objects with the same ID as this object.

    public List<ActivatableObject> GetActivatable { // Objects that can be activated by this ID.
        get {
            if (linkedObjects == null) ConfigureLinkedObjects();
            return linkedObjects.Where(x => x is not ActivatorObject).ToList();
        }
    }

    public List<ActivatorObject> GetActivators { // Objects that need to be activated to activate this ID.
        get {
            if (linkedObjects == null) ConfigureLinkedObjects();
            return linkedObjects.OfType<ActivatorObject>().ToList();
        }
    }
    
    public void ConfigureLinkedObjects() {
        // Get the linked object of this ID. If none, create a new list.
        linkedObjects = FindObjectsOfType<ActivatableObject>().FirstOrDefault(x => x.id == id && x != this)?.linkedObjects ?? new List<ActivatableObject>();
        linkedObjects.Add(this);
    }
    
    protected virtual void Awake() {
        base.Awake();
        ConfigureLinkedObjects(); // TODO: May not be needed.
        // Debug.Log("Created " + gameObject.name + ". Game contains " +  linkedObjects.Count + " objects already with ID " + id);
    }
    
    public virtual void UpdateActivation() { // Must be called when the state of the activator objects change. (Separate function to account for animations)
        // for all Activators, if they are all active, call the Activate() method on objects that are Activatable.
        bool allActive = GetActivators.TrueForAll(x => x.isActive);
        if (allActive != _lastActiveState) {
            GetActivatable.ForEach(x => x.OnActiveChange(allActive));
            _lastActiveState = allActive;
        }
    }
    
    protected virtual void Activate() {
        isActive = !outputInverted;
    }
    
    protected virtual void Deactivate() {
        isActive = outputInverted;
    }

    protected virtual void OnActiveChange(bool activate) {throw new NotImplementedException();}
}

[CustomEditor(typeof(ActivatableObject), true), CanEditMultipleObjects]
public class ActivatableObjectEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        ActivatableObject ao = (ActivatableObject) target;
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.LabelField("Activators:");
        foreach (ActivatableObject activatable in FindObjectsOfType<ActivatableObject>().OfType<ActivatorObject>().Where(x => x.id == ao.id).Reverse()) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(activatable.isActive ? "✓" : "✖", GUILayout.Width(20));
            EditorGUILayout.ObjectField(activatable.gameObject, typeof(GameObject), true);
            GUILayout.EndHorizontal();
        }
        
        EditorGUILayout.LabelField("Activatables:");
        foreach (ActivatableObject activatable in FindObjectsOfType<ActivatableObject>().Where(x => x is not ActivatorObject).Reverse()) {
            if (activatable.id == ao.id) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(activatable.isActive ? "✓" : "✖", GUILayout.Width(20));
                EditorGUILayout.ObjectField(activatable.gameObject, typeof(GameObject), true);
                GUILayout.EndHorizontal();
            }
        }
        EditorGUI.EndDisabledGroup();
    }
}