using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public Button nextButton;
    public float typingSpeed = 0.05f;

    private string[] steps = {
        "Chào mừng đến với Snowman is You!",
        "Đẩy các khối chữ để tạo thành quy tắc có nghĩa.",
        "Ví dụ: SNOWMAN IS YOU cho phép bạn điều khiển Snowman.",
        "Tuy nhiên, nếu bạn phá vỡ các cấu trúc, quy tắc tương ứng sẽ không xảy ra.",
        "Dọn sạch tuyết để hoàn thành màn chơi. Chúc may mắn!"
    };

    private int currentStep = 0;

    void Start()
    {
        nextButton.onClick.AddListener(OnNextClicked);
        nextButton.gameObject.SetActive(false);
        StartCoroutine(TypeText(steps[currentStep]));
    }

    IEnumerator TypeText(string text)
    {
        tutorialText.text = "";
        nextButton.gameObject.SetActive(false);
        foreach (char c in text)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        nextButton.gameObject.SetActive(true);
    }

    void OnNextClicked()
    {
        currentStep++;
        if (currentStep < steps.Length)
            StartCoroutine(TypeText(steps[currentStep]));
        else
            tutorialPanel.SetActive(false);
    }
}