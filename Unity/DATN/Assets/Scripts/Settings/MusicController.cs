using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // giữ lại khi đổi scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusic(bool isOn)
    {
        if (isOn)
        {
            if (!audioSource.isPlaying)
            {
                // Nếu đang pause thì tiếp tục, nếu chưa phát thì phát mới
                audioSource.UnPause();
                if (audioSource.time == 0f) audioSource.Play();
            }
        }
        else
        {
            audioSource.Pause(); // dừng tạm, giữ vị trí hiện tại
        }
    }
}
