using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlock : MonoBehaviour
{
    public int unlockedLevel = 1; // nhập số level mở ở đây (ví dụ 3)

    void Start()
    {
        for (int i = 1; i <= unlockedLevel; i++)
        {
            // Tìm GameObject theo tên (ví dụ "Level1", "Level2"...)
            GameObject levelObj = GameObject.Find("Level" + i);

            if (levelObj != null)
            {
                Button btn = levelObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = true; // mở nút
                }
            }
        }

        // Các level sau unlockedLevel sẽ bị khóa
        int totalLevels = 5; // số level tổng cộng, bạn chỉnh theo game
        for (int i = unlockedLevel + 1; i <= totalLevels; i++)
        {
            GameObject levelObj = GameObject.Find("Level" + i);

            if (levelObj != null)
            {
                Button btn = levelObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = false; // khóa nút
                }
            }
        }
    }
}