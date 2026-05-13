using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    public Image innerBar;   // gán InnerBar sprite (có bóng ở đầu)
    public float speed = 0.3f;
    private float progress = 0f;

    void Update()
    {
        progress += speed * Time.deltaTime;
        // Tăng progress tự động
        progress += speed * Time.deltaTime;
        if (progress > 1f)
        {
            progress = 1f;
            // Khi đầy thì chuyển scene
            SceneManager.LoadScene("StartScene");
        }

        // Thanh chạy
        innerBar.fillAmount = progress;
    }
}