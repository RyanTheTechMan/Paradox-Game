using System;
using UnityEngine;

public class ResolutionChangeEvent : MonoBehaviour {
    public static event Action onResolutionChangedStarted;
    public static event Action onResolutionChangedEnded;
    
    private static ResolutionChangeEvent instance;
    private static bool resolutionChanging;
    private static Vector2 _screenSize; // Used for testing OnResolutionChange
    private static float _timeSinceLastChange = 0f;
    private const float _waitTime = 0.5f; // Wait time before resolution change is completed.

    public GameObject textOverlay;
    private GameObject _textOverlay;

    private void Awake()
    {
        if (instance != null) {
            Debug.LogWarning("There can only be one ResolutionChangeEvent in the scene.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        _screenSize.x = Screen.width;
        _screenSize.y = Screen.height;
    }

    private void Update()
    {
        // When the resolution changes, call OnResolutionChanged.
        if (_screenSize.x != Screen.width || _screenSize.y != Screen.height)
        {
            if (!resolutionChanging)
            {
                resolutionChanging = true;
                onResolutionChangedStarted?.Invoke();
                _textOverlay ??= Instantiate(textOverlay, transform);
            }
            _timeSinceLastChange = 0f;
        }
        else
        {
            if (resolutionChanging)
            {
                if (_timeSinceLastChange >= _waitTime)
                {
                    resolutionChanging = false;
                    _timeSinceLastChange = 0f;
                    onResolutionChangedEnded?.Invoke();
                    if (_textOverlay != null)
                    {
                        Destroy(_textOverlay);
                        _textOverlay = null;
                    }
                }
            }
            
            _timeSinceLastChange += Time.deltaTime;
        }

        _screenSize.x = Screen.width;
        _screenSize.y = Screen.height;
    }
}