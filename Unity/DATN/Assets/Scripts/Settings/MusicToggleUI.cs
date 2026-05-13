using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicToggleUI : MonoBehaviour
{
    public Toggle toggle;

    void Start()
    {
        toggle.isOn = MusicManager.Instance.audioSource.isPlaying;
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        MusicManager.Instance.SetMusic(isOn);
    }
}
