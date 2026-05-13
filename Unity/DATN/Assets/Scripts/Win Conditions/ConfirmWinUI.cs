using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmWinUI : MonoBehaviour
{
    public Button btnConfirm; // "Màn tiếp"
    public Button btnCancel;  // "Chơi tiếp"

    void Start()
    {
        btnConfirm.onClick.AddListener(OnConfirm);
        btnCancel.onClick.AddListener(OnCancel);
    }

    void OnConfirm()
    {
        LevelScore.Instance?.Calculate();
        SceneManager.UnloadSceneAsync("ConfirmWinScreen");
        if (GridPlayer.Instance != null)
            GridPlayer.Instance.gameObject.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("WinScene", LoadSceneMode.Additive);
    }

    void OnCancel()
    {
        SceneManager.UnloadSceneAsync("ConfirmWinScreen");
        Time.timeScale = 1f;

        if (SnowWinTrigger.Instance != null)
            SnowWinTrigger.Instance.ShowSkipButton();
    }
}