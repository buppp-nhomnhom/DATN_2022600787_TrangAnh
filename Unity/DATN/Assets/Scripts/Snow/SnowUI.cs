using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SnowUI : MonoBehaviour
{
    public TextMeshProUGUI percentText;

    void Update()
    {
        if (SnowManager.Instance == null) return;
        float percent = SnowManager.Instance.ClearedPercent;
        percentText.text = $"Tuyết: {percent:F0}%";
    }
}