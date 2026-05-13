using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsUI : MonoBehaviour
{
    public void OpenSettingsInGame()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("SettingsInGame", LoadSceneMode.Additive);
    }
}