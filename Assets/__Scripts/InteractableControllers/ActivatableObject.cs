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
            return linkedObjects.Where(x => x is ActivatorObject).Cast<ActivatorObject>().ToList();
        }
    }
    
    public void ConfigureLinkedObjects() {
        foreach (ActivatableObject ao in FindObjectsOfType<ActivatableObject>()) {
            if (ao != this) linkedObjects = ao.linkedObjects;
        }
        linkedObjects ??= new List<ActivatableObject>();
        linkedObjects.Add(this);
    }
    
    protected new void Awake() {
        ConfigureLinkedObjects(); // TODO: May not be needed.
        // Debug.Log("Created " + gameObject.name + ". Game contains " +  linkedObjects.Count + " objects already with ID " + id);
    }
    
    public virtual void UpdateActivation() {
        // for all objects in linkedObjects, if any are not active, call the Activate() method on objects that are not ActivatorObjects.
        if (GetActivators.TrueForAll(x => x.isActive)) {
            GetActivatable.ForEach(x => x.Activate());
        }
    }
    
    protected void Activate() {
        isActive = !outputInverted;
        UpdateActivation();
    }
    
    protected void Deactivate() {
        isActive = outputInverted;
        UpdateActivation();
    }
}

[CustomEditor(typeof(ActivatableObject), true)]
public class ActivatableObjectEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        ActivatableObject ao = (ActivatableObject) target;
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.LabelField("Activators:");
        foreach (ActivatableObject activatable in FindObjectsOfType<ActivatableObject>().Where(x => x is ActivatorObject).Reverse()) {
            if (activatable.id == ao.id) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(activatable.isActive ? "✓" : "✖", GUILayout.Width(20));
                EditorGUILayout.ObjectField(activatable.gameObject, typeof(GameObject), true);
                GUILayout.EndHorizontal();
            }
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