using UnityEngine;

public class GateController : MonoBehaviour
{
    [Header("Chọn 1 trong 2 cách")]
    public Animator gateAnimator;  // Cách 1: dùng Animator
    public Transform gatePivot;    // Cách 2: không có Animator
    public float openAngle = 90f;
    public float openDuration = 1.2f;

    bool isOpen = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isOpen) return;

        bool canOpen = (GridPlayer.Instance.hasFinalKey && RuleManager.Instance.IsOpen("Key"))
                    || RuleManager.Instance.IsOpen("Snowman");

        if (canOpen)
        {
            isOpen = true;
            OpenGate();
        }
        else
        {
            Debug.Log("Cần key hoặc Snowman is Open!");
        }
    }

    public GameObject gateBlocker; // invisible wall trước cửa

    void OpenGate()
    {
        SoundManager.Instance?.PlayOpenGate();
        if (gateBlocker != null) gateBlocker.SetActive(false);

        if (gateAnimator != null)
            gateAnimator.SetTrigger("Open");
        else if (gatePivot != null)
            StartCoroutine(RotateGate());
    }

    System.Collections.IEnumerator RotateGate()
    {
        float elapsed = 0f;
        Quaternion startRot = gatePivot.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, openAngle, 0);

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / openDuration);
            gatePivot.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        gatePivot.rotation = endRot;
        GetComponent<Collider>().enabled = false;
    }
}