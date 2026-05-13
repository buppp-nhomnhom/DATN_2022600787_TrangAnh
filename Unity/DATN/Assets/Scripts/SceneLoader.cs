using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("LevelMapScene"); // chuyển sang scene Level Map
    }

    public void LoadSettings()
    {
        // Load SettingsScene chồng lên MenuScene (overlay)
        SceneManager.LoadScene("SettingsScene");
    }
    public void OnCloseButton()
    {
        // Load lại scene StartScene
        SceneManager.LoadScene("StartScene");
    }

    public void QuitGame()
    {
        Application.Quit(); // thoát game
        Debug.Log("Game exited!");
    }

    //Load level kế tiếp
    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }
    public void LoadNextLevelWithFinalKey()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        if (GridPlayer.Instance != null && GridPlayer.Instance.hasFinalKey)
        {
            SceneManager.LoadScene(currentIndex + 1);
        }
        else
        {
            Debug.Log("❌ Bạn chưa có FinalKey để mở màn này!");
            // Nếu muốn, gọi UI popup ở đây
        }
    }

}