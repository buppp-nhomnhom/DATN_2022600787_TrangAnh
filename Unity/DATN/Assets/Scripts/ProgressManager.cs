using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    // Hàm mở khóa level mới
    public static void UnlockLevel(int levelIndex)
    {
        int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (levelIndex > currentUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", levelIndex);
            PlayerPrefs.Save();
            Debug.Log("UnlockedLevel updated to " + levelIndex);
        }
    }

    // Hàm gọi khi thắng màn chơi hiện tại
    public static void LevelCompleted(int currentLevel)
    {
        int nextLevel = currentLevel + 1;
        UnlockLevel(nextLevel);
    }

    // Hàm reset tiến trình (chỉ mở lại Level1)
    public static void ResetProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
        Debug.Log("Progress reset to Level 1");
    }
}