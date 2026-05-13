using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreenUI : MonoBehaviour
{
    [Header("Text số liệu")]
    public TextMeshProUGUI txtScore;        // kéo ScoreValue vào
    public TextMeshProUGUI txtTotalScore;   // kéo TotalScoreValue vào
    public TextMeshProUGUI txtSnowPercent;  // kéo SnowPercentValue vào

    [Header("Buttons")]
    public Button btnReplay;    // nút Chơi lại
    public Button btnNextLevel; // nút Màn tiếp
    public Button btnQuit;      // nút Thoát

    void Start()
    {
        // Điền số liệu vào Text
        txtScore.text = LevelScore.LastScore.ToString();
        txtTotalScore.text = GameData.GetTotalScore().ToString();
        txtSnowPercent.text = LevelScore.LastPercent.ToString("F0") + "%";

        // Gán sự kiện cho các nút
        btnReplay.onClick.AddListener(OnReplay);
        btnNextLevel.onClick.AddListener(OnNextLevel);
        btnQuit.onClick.AddListener(OnQuit);
    }

    void OnReplay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(LevelScore.CurrentLevelName);
    }

    void OnNextLevel()
    {
        Time.timeScale = 1f;

        // Mở khóa level tiếp theo
        ProgressManager.LevelCompleted(LevelScore.LastLevelIndex);

        // Tự động tính tên scene tiếp theo
        // Level1 → Level2, Level2 → Level3...
        int nextIndex = LevelScore.LastLevelIndex + 1;
        string nextScene = "Level" + nextIndex;
        SceneManager.LoadScene(nextScene);
    }

    void OnQuit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelMapScene");
    }
}