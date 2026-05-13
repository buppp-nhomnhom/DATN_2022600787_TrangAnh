using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Button btnContinue; // ← thêm field này

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        btnContinue.gameObject.SetActive(unlockedLevel > 1);
    }

    public void StartGame()
    {
        GameData.ResetAll();
        ProgressManager.ResetProgress();
        SceneManager.LoadScene("Level1");
    }

    public void ContinueGame()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        SceneManager.LoadScene("Level" + unlockedLevel);
    }

    public void OpenSettings()
    {
        Debug.Log("Settings clicked!");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited!");
    }
}