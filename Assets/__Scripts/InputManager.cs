using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    public PlayerControls playerControls;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        playerControls = new PlayerControls();
    }
    
    private void OnEnable()
    {
        playerControls?.Enable();
    }
    
    private void OnDisable()
    {
        playerControls?.Disable();
    }
}
