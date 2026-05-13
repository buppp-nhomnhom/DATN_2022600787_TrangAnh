using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoseScreenUI : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI txtScore;
    public TextMeshProUGUI txtTotalScore;
    public TextMeshProUGUI txtSnowPercent;

    [Header("Buttons")]
    public Button btnReplay;
    public Button btnQuit;

    void Start()
    {
        txtScore.text = $"{LevelScore.LastScore}";
        txtTotalScore.text = $"{GameData.GetTotalScore()}";
        txtSnowPercent.text = $"{LevelScore.LastPercent:F0}%";

        btnReplay.onClick.AddListener(OnReplay);
        btnQuit.onClick.AddListener(OnQuit);
    }

    void OnReplay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(LevelScore.CurrentLevelName);
    }

    void OnQuit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelMapScene");
    }
}