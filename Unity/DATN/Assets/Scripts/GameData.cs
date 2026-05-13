using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    // Keys
    const string KEY_TOTAL_SCORE = "TotalScore";
    const string KEY_RUNE_PREFIX = "Rune_Level_";
    const string KEY_LEVEL_SCORE_PREFIX = "Score_Level_";
    const string KEY_HIDDEN_LEVEL_UNLOCKED = "HiddenLevelUnlocked";

    // Tổng điểm
    public static int GetTotalScore()
        => PlayerPrefs.GetInt(KEY_TOTAL_SCORE, 0);

    public static void AddScore(int score)
    {
        int current = GetTotalScore();
        PlayerPrefs.SetInt(KEY_TOTAL_SCORE, current + score);
        PlayerPrefs.Save();
    }

    // Điểm từng level
    public static int GetLevelScore(int levelIndex)
        => PlayerPrefs.GetInt(KEY_LEVEL_SCORE_PREFIX + levelIndex, 0);

    public static void SaveLevelScore(int levelIndex, int score)
    {
        // Chỉ lưu nếu điểm cao hơn lần trước
        int prev = GetLevelScore(levelIndex);
        if (score > prev)
        {
            int diff = score - prev;
            PlayerPrefs.SetInt(KEY_LEVEL_SCORE_PREFIX + levelIndex, score);
            AddScore(diff); // chỉ cộng phần chênh lệch vào tổng
        }
        PlayerPrefs.Save();
    }

    // Special Rune
    public static bool HasRune(int levelIndex)
        => PlayerPrefs.GetInt(KEY_RUNE_PREFIX + levelIndex, 0) == 1;

    public static void SaveRune(int levelIndex)
    {
        PlayerPrefs.SetInt(KEY_RUNE_PREFIX + levelIndex, 1);
        PlayerPrefs.Save();
        CheckUnlockHiddenLevel();
    }

    public static int GetRuneCount()
    {
        int count = 0;
        for (int i = 1; i <= 4; i++)
            if (HasRune(i)) count++;
        return count;
    }

    // Level ẩn
    public static bool IsHiddenLevelUnlocked()
        => PlayerPrefs.GetInt(KEY_HIDDEN_LEVEL_UNLOCKED, 0) == 1;

    static void CheckUnlockHiddenLevel()
    {
        if (GetRuneCount() >= 4)
        {
            PlayerPrefs.SetInt(KEY_HIDDEN_LEVEL_UNLOCKED, 1);
            PlayerPrefs.Save();
            Debug.Log("[GameData] Hidden level unlocked!");
        }
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
