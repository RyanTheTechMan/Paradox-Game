using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    
    private Vector2 _screenSize; // Used for testing OnResolutionChange
    delegate void OnResolutionChanged();
    OnResolutionChanged onResolutionChanged;
    [NonSerialized]
    public bool resolutionChanging = false;
    public float timeSinceLastChange 

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        
        _screenSize.x = Screen.width;
        _screenSize.y = Screen.height;
    }
    
    
    private void Update()
    {
        // When the resolution changes, call OnResolutionChanged.
        if (_screenSize.x != Screen.width || _screenSize.y != Screen.height)
        {
            resolutionChanging = true;
            onResolutionChanged?.Invoke();
        }
        

        _screenSize.x = Screen.width;
        _screenSize.y = Screen.height;
    }
}