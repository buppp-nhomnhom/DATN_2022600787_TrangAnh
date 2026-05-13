using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RunePopupUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject[] runePrefabs; // kéo 4 prefab rune vào theo thứ tự level
    public Transform runeDisplay;    // vị trí hiện rune 3D
    public TextMeshProUGUI runeName; // tên rune

    string[] runeNames = { "Cá Đuối", "Cá Nục", "Hải Cẩu", "Cá Mập" };

    GameObject currentRune;

    void Start()
    {
        Debug.Log($"LastLevelIndex = {LevelScore.LastLevelIndex}");
        int levelIndex = Mathf.Clamp(LevelScore.LastLevelIndex - 1, 0, 3);
        Debug.Log($"levelIndex = {levelIndex}");

        if (runePrefabs[levelIndex] != null)
        {
            currentRune = Instantiate(
                runePrefabs[levelIndex],
                runeDisplay.position,
                Quaternion.identity);

            currentRune.transform.localRotation = Quaternion.Euler(0, 90, 45);

            // Set layer Rune cho model
            currentRune.layer = LayerMask.NameToLayer("Rune");
            foreach (Transform child in currentRune.transform)
                child.gameObject.layer = LayerMask.NameToLayer("Rune");
        }

        if (runeName != null)
            runeName.text = runeNames[levelIndex];
    }

    void Update()
    {
        // Xoay rune 3D
        if (currentRune != null)
            currentRune.transform.Rotate(0, 90f * Time.unscaledDeltaTime, 0);
    }

    // Nút "Chơi tiếp"
    public void OnChơiTiếp()
    {
        SceneManager.UnloadSceneAsync("RunePopup");
        Time.timeScale = 1f;
        // Nút Bỏ qua vẫn còn
    }

    // Nút "Màn tiếp"
    public void OnMànTiếp()
    {
        LevelScore.Instance?.Calculate();
        if (GridPlayer.Instance != null)
            GridPlayer.Instance.gameObject.SetActive(false);
        SceneManager.UnloadSceneAsync("RunePopup");
        Time.timeScale = 1f;
        SceneManager.LoadScene("WinScene", LoadSceneMode.Additive);
    }
}