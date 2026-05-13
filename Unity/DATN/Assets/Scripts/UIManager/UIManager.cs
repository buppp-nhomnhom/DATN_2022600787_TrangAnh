using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject needKeyPanel;

    public void ShowNeedFinalKeyPopup()
    {
        needKeyPanel.SetActive(true);
    }

    public void HideNeedFinalKeyPopup()
    {
        Debug.Log("👉 Hàm HideNeedFinalKeyPopup được gọi!");
        needKeyPanel.SetActive(false);
    }
}