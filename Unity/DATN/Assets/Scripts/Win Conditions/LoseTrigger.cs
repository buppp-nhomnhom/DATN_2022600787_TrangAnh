using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseTrigger : MonoBehaviour
{
    public string loseSceneName = "LoseScreen";
    private bool triggered = false;
    private bool readyToCheck = false;

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(2f);
        readyToCheck = true;
    }

    void Update()
    {
        if (!readyToCheck) return;
        if (triggered) return;
        if (RuleManager.Instance == null) return;

        if (!RuleManager.Instance.IsYou("Snowman"))
        {
            triggered = true;
            StartCoroutine(TriggerLose());
        }
    }

    IEnumerator TriggerLose()
    {
        yield return new WaitForSeconds(1.5f);
        LevelScore.Instance.Calculate();
        Time.timeScale = 0f;
        SceneManager.LoadScene(loseSceneName, LoadSceneMode.Additive);
    }
}