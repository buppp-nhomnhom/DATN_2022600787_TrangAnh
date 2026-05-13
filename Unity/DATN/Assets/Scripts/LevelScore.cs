using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScore : MonoBehaviour
{
    public static LevelScore Instance;

    [Header("Level Info")]
    public int levelIndex = 1;
    public string levelName; // tên scene level này, VD: "Level1"

    [Header("Rune Threshold")]
    public float runeThreshold = 90f;

    // Static để WinScene đọc được sau khi load
    public static int LastScore { get; private set; }
    public static float LastPercent { get; private set; }
    public static string CurrentLevelName { get; private set; }
    public static int LastLevelIndex { get; private set; }

    public int Score { get; private set; }
    public bool EarnedRune { get; private set; }

    void Awake()
    {
        Instance = this;
        CurrentLevelName = levelName;
    }

    public void Calculate()
    {
        LastLevelIndex = levelIndex;
        float percent = SnowManager.Instance != null
            ? SnowManager.Instance.ClearedPercent : 0f;

        Score = Mathf.RoundToInt(percent);
        LastScore = Score;
        LastPercent = percent;

        GameData.SaveLevelScore(levelIndex, Score);

        EarnedRune = percent >= runeThreshold;
        if (EarnedRune)
            GameData.SaveRune(levelIndex);

        Debug.Log($"[LevelScore] Level {levelIndex}: {Score} điểm, Rune={EarnedRune}");
    }
}