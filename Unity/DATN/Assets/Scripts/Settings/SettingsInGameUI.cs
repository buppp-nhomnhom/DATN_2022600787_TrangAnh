using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsInGameUI : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle soundToggle;
    public Toggle vibrationToggle;

    void Start()
    {
        musicToggle.isOn = PlayerPrefs.GetInt("music", 1) == 1;
        soundToggle.isOn = PlayerPrefs.GetInt("sound", 1) == 1;
        vibrationToggle.isOn = PlayerPrefs.GetInt("vibration", 1) == 1;

        musicToggle.onValueChanged.AddListener(v => PlayerPrefs.SetInt("music", v ? 1 : 0));
        soundToggle.onValueChanged.AddListener(v => PlayerPrefs.SetInt("sound", v ? 1 : 0));
        vibrationToggle.onValueChanged.AddListener(v => PlayerPrefs.SetInt("vibration", v ? 1 : 0));
    }
    public void OpenSettingsInGame()
    {
        Time.timeScale = 0f; // pause game
        SceneManager.LoadScene("SettingsInGame", LoadSceneMode.Additive);
    }
    // Button "Chơi tiếp"
    public void ResumeGame()
    {
        SceneManager.UnloadSceneAsync("SettingsInGame");
        Time.timeScale = 1f; // tiếp tục game
    }
    // Button "Chơi lại"
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Button "Thoát" → quay về LevelMapScene
    public void ExitToLevelMap()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelMapScene");
    }
}