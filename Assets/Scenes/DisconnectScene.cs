using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectScene : MonoBehaviour
{
    private void Start()
    {
        // Global.Instance.SelectCanvas.gameObject.SetActive(false);
        Global.Instance.LobbyCanvas.gameObject.SetActive(false);
        
        Global.Instance.IngameActivingCursor = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("Title");
    }
}
