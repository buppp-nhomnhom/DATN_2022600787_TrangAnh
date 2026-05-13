using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnowWinTrigger : MonoBehaviour
{
    public static SnowWinTrigger Instance;

    [Header("Threshold")]
    public float triggerAt = 40f;
    public float runeThreshold = 90f;

    [Header("Nút Bỏ qua")]
    public GameObject skipButtonGO;

    bool triggered40 = false;
    bool triggered90 = false;

    void Awake()
    {
        Instance = this;
        skipButtonGO.SetActive(false);
    }

    void Update()
    {
        if (SnowManager.Instance == null) return;
        float percent = SnowManager.Instance.ClearedPercent;

        // Trigger 40%
        if (!triggered40 && percent >= triggerAt)
        {
            triggered40 = true;
            Time.timeScale = 0f;
            SceneManager.LoadScene("ConfirmWinScreen", LoadSceneMode.Additive);
        }

        // Trigger 90% — chỉ khi đã chọn "Chơi tiếp"
        if (triggered40 && !triggered90 && percent >= runeThreshold)
        {
            triggered90 = true;
            LevelScore.Instance?.Calculate(); // ← thêm dòng này
            Time.timeScale = 0f;
            SceneManager.LoadScene("RunePopup", LoadSceneMode.Additive);
        }
    }

    public void ShowSkipButton()
    {
        skipButtonGO.SetActive(true);
    }

    public void OnClickSkip()
    {
        LevelScore.Instance?.Calculate();
        if (GridPlayer.Instance != null)
            GridPlayer.Instance.gameObject.SetActive(false);
        skipButtonGO.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("WinScene", LoadSceneMode.Additive);
    }
}