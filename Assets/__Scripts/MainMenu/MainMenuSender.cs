using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSender : MonoBehaviour {
    public static MainMenuSender Instance;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        
        DontDestroyOnLoad(gameObject);

        InputManager.Instance.playerControls.FirstPerson.PauseMenu.performed += ctx => SendToMainMenu();
    }

    public static void SendToMainMenu() {
        if (PlayerController.Instance)
            DestroyImmediate(PlayerController.Instance.gameObject);
        if (LevelManager.Instance && LevelManager.tempLoadRoom)
            DestroyImmediate(LevelManager.tempLoadRoom.gameObject);
        if (GameManager.Instance)
            DestroyImmediate(GameManager.Instance.gameObject);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
}
