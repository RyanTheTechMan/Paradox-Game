using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderManager : MonoBehaviour
{
    private void Awake()
    {
        int LeftViewLayer = LayerMask.NameToLayer("_LeftView");
        int RightViewLayer = LayerMask.NameToLayer("_RightView");
        
        int LeftEye = LayerMask.NameToLayer("Left Eye");
        int RightEye = LayerMask.NameToLayer("Right Eye");
        
        // For each gameobject
        foreach (GameObject go in FindObjectsOfType(typeof(GameObject)))
        {
            if (go.layer == LeftEye) go.layer = LeftViewLayer;
            if (go.layer == RightEye) go.layer = RightViewLayer;
        }
    }
}
