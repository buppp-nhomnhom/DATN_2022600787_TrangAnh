using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggleUI : MonoBehaviour
{
    public Toggle toggle;

    void Start()
    {
        toggle.isOn = SoundManager.Instance.IsSoundEnabled();
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        SoundManager.Instance.SetSound(isOn);
    }
}