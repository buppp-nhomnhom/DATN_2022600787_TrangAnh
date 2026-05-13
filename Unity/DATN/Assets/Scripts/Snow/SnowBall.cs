using UnityEngine;
using System.Collections;

public class SnowBall : MonoBehaviour
{
    public static SnowBall Instance { get; private set; }

    [Header("Size")]
    public float minSize = 0.3f;  // to hơn lúc spawn (từ 0.1 lên 0.3)
    public float maxSize = 1.5f;  // to hơn lúc max (từ 0.4 lên 1.5)
    public float growSpeed = 0.03f;

    [Header("Roll")]
    public float rollSpeed = 6f;

    [Header("Pop")]
    public float popDuration = 0.3f;
    public GameObject popEffect;

    float currentSize;
    bool isPopping = false;
    bool isDead = false;
    Transform player;
    Terrain terrain; // ← thêm reference terrain

    void Awake()
    {
        Instance = this;
        currentSize = minSize;
        UpdateSize();
    }

    // Thêm terrain vào Init
    public void Init(Transform playerTransform, Terrain t)
    {
        player = playerTransform;
        terrain = t;
    }

    public void Grow()
    {
        if (isPopping || isDead) return;
        currentSize += growSpeed * Time.deltaTime;
        UpdateSize();
    }

    public void ForcePop()
    {
        if (isPopping || isDead) return;
        StartCoroutine(DoPop());
    }

    void Update()
    {
        if (isPopping || isDead || player == null) return;

        // Vị trí phía trước player
        Vector3 targetPos = player.position + player.forward * 1.5f; // khớp với spawnPos

        // Dùng terrain.SampleHeight để bám đất chính xác
        if (terrain != null)
        {
            float groundY = terrain.SampleHeight(targetPos)
                          + terrain.transform.position.y;
            targetPos.y = groundY + currentSize * 0.5f; // nửa bóng nổi trên mặt đất
        }

        transform.position = Vector3.Lerp(
            transform.position, targetPos, rollSpeed * Time.deltaTime);
    }

    void UpdateSize()
    {
        transform.localScale = Vector3.one * currentSize;
    }

    IEnumerator DoPop()
    {
        isPopping = true;
        isDead = true;

        float t = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 2f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(
                startScale, endScale, t / popDuration);
            yield return null;
        }

        if (popEffect != null)
            Instantiate(popEffect, transform.position, Quaternion.identity);

        Instance = null;
        SnowManager.Instance?.OnSnowballPopped();
        Destroy(gameObject);
    }
}