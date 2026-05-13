using UnityEngine;

public class SnowManager : MonoBehaviour
{
    public static SnowManager Instance;

    [Header("Terrain")]
    public Terrain terrain;
    public Material snowMaterial;
    public int textureSize = 1024;

    [Header("Zones")]
    public Collider[] snowZoneColliders;
    public Collider heavySnowZoneCollider;
    public Collider[] forbiddenZones;

    [Header("Brush")]
    public float lightBrushSize = 1.5f;
    public float heavyBrushSize = 2.5f;

    [Header("Snowball")]
    public GameObject snowballPrefab;
    public Transform player;
    public float feedInterval = 0.1f;

    [Header("Snow Check")]
    public float checkInterval = 0.5f;

    RenderTexture snowMaskRT;
    Material paintMat;
    float terrainSizeX, terrainSizeZ;
    float lastFeedTime;
    float lastCheckTime = 0f;

    float totalPixelsInitial = 0f;
    float heavyPixelsInitial = 0f;
    float heavyRemaining = -1f;
    float totalRemaining = -1f;

    public float ClearedPercent
    {
        get
        {
            if (totalPixelsInitial <= 0f) return 0f;
            if (totalRemaining < 0f) return 0f;
            return Mathf.Clamp01(1f - totalRemaining / totalPixelsInitial) * 100f;
        }
    }

    bool snowballAlive = false;
    bool heavyAllCleared = false;
    bool playerHasShovel = false;
    bool snowballPopped = false;

    void Awake()
    {
        Instance = this;

        snowMaskRT = new RenderTexture(
            textureSize, textureSize, 0, RenderTextureFormat.RFloat);
        snowMaskRT.filterMode = FilterMode.Bilinear;
        snowMaskRT.Create();
        Graphics.Blit(Texture2D.whiteTexture, snowMaskRT);

        paintMat = new Material(Shader.Find("Hidden/SnowPaint"));
        snowMaterial.SetTexture("_SnowMask", snowMaskRT);

        terrainSizeX = terrain.terrainData.size.x;
        terrainSizeZ = terrain.terrainData.size.z;
    }

    void Start()
    {
        CountInitialPixels();
        heavyAllCleared = false;
        snowballAlive = false;
        heavyRemaining = heavyPixelsInitial;
        totalRemaining = totalPixelsInitial;
    }

    void Update()
    {
        if (heavySnowZoneCollider != null)
        {
            Bounds b = heavySnowZoneCollider.bounds;
            snowMaterial.SetVector("_HeavyZoneMin",
                new Vector4(b.min.x, 0, b.min.z, 0));
            snowMaterial.SetVector("_HeavyZoneMax",
                new Vector4(b.max.x, 0, b.max.z, 0));
        }
    }

    public void SetHasShovel(bool val) => playerHasShovel = val;

    bool IsInAnySnowZone(Vector3 worldPos)
    {
        foreach (Collider zone in snowZoneColliders)
            if (zone != null && zone.bounds.Contains(worldPos))
                return true;
        return false;
    }

    bool IsInForbiddenZone(Vector3 worldPos)
    {
        if (forbiddenZones == null) return false;
        foreach (Collider zone in forbiddenZones)
            if (zone != null && zone.bounds.Contains(worldPos))
                return true;
        return false;
    }

    public void TrackPosition(Vector3 worldPos, bool isMoving)
    {
        if (!IsInAnySnowZone(worldPos)) return;

        bool inHeavy = IsInZone(worldPos, heavySnowZoneCollider);

        if (inHeavy)
        {
            bool canClear = (playerHasShovel || RuleManager.Instance.IsYou("Shovel"))
                && RuleManager.Instance.IsClear("Shovel");
            if (!canClear || !isMoving) return;

            Paint(worldPos, heavyBrushSize, true);
            CheckSnowRemaining();

            if (!snowballPopped && heavyRemaining >= 0f
                && heavyRemaining < heavyPixelsInitial * 0.01f)
            {
                snowballPopped = true;
                heavyAllCleared = true;
                if (SnowBall.Instance != null)
                    SnowBall.Instance.ForcePop();
                return;
            }

            if (!snowballPopped)
                SpawnOrUpdateSnowball(worldPos);
        }
        else
        {
            if (!isMoving) return;
            Paint(worldPos, lightBrushSize, false);
            CheckSnowRemaining();
        }
    }

    const int GRID_RES = 300;

    void CountInitialPixels()
    {
        for (int x = 0; x < GRID_RES; x++)
        {
            for (int z = 0; z < GRID_RES; z++)
            {
                Vector3 worldPos = terrain.transform.position + new Vector3(
                    (float)x / GRID_RES * terrainSizeX,
                    0,
                    (float)z / GRID_RES * terrain.terrainData.size.z);

                if (IsInForbiddenZone(worldPos)) continue;

                if (IsInAnySnowZone(worldPos))
                {
                    totalPixelsInitial++;
                    if (IsInZone(worldPos, heavySnowZoneCollider))
                        heavyPixelsInitial++;
                }
            }
        }
    }

    void CheckSnowRemaining()
    {
        if (Time.time - lastCheckTime < checkInterval) return;
        lastCheckTime = Time.time;

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = snowMaskRT;
        Texture2D tex = new Texture2D(
            snowMaskRT.width, snowMaskRT.height, TextureFormat.RFloat, false);
        tex.ReadPixels(
            new Rect(0, 0, snowMaskRT.width, snowMaskRT.height), 0, 0);
        tex.Apply();
        RenderTexture.active = prev;

        float heavyLeft = 0f;
        float totalLeft = 0f;

        for (int x = 0; x < GRID_RES; x++)
        {
            for (int z = 0; z < GRID_RES; z++)
            {
                Vector3 worldPos = terrain.transform.position + new Vector3(
                    (float)x / GRID_RES * terrainSizeX,
                    0,
                    (float)z / GRID_RES * terrain.terrainData.size.z);

                if (IsInForbiddenZone(worldPos)) continue;

                int px = Mathf.Clamp(
                    Mathf.RoundToInt((float)x / GRID_RES * snowMaskRT.width),
                    0, snowMaskRT.width - 1);
                int py = Mathf.Clamp(
                    Mathf.RoundToInt((float)z / GRID_RES * snowMaskRT.height),
                    0, snowMaskRT.height - 1);

                float val = tex.GetPixel(px, py).r;

                if (IsInAnySnowZone(worldPos))
                {
                    totalLeft += val;
                    if (IsInZone(worldPos, heavySnowZoneCollider))
                        heavyLeft += val;
                }
            }
        }

        Destroy(tex);
        heavyRemaining = heavyLeft;
        totalRemaining = totalLeft;
    }

    void Paint(Vector3 worldPos, float brushSize, bool isHeavy)
    {
        Vector3 local = worldPos - terrain.transform.position;
        paintMat.SetVector("_BrushPos", new Vector4(
            local.x / terrainSizeX,
            local.z / terrain.terrainData.size.z, 0, 0));
        paintMat.SetFloat("_BrushSize", brushSize / terrainSizeX);
        paintMat.SetFloat("_BrushValue", 0f);

        RenderTexture temp = RenderTexture.GetTemporary(snowMaskRT.descriptor);
        Graphics.Blit(snowMaskRT, temp, paintMat);
        Graphics.Blit(temp, snowMaskRT);
        RenderTexture.ReleaseTemporary(temp);

        if (isHeavy)
            SoundManager.Instance?.PlayHeavySnow();
        else
            SoundManager.Instance?.PlayLightSnow();
    }

    void SpawnOrUpdateSnowball(Vector3 playerPos)
    {
        if (heavyAllCleared || snowballPopped) return;

        if (SnowBall.Instance != null)
        {
            SnowBall.Instance.Grow();
        }
        else if (!snowballAlive)
        {
            if (Time.time - lastFeedTime < feedInterval) return;
            lastFeedTime = Time.time;

            Vector3 spawnPos = playerPos + player.forward * 1.5f;
            spawnPos.y = terrain.SampleHeight(spawnPos)
                       + terrain.transform.position.y;

            GameObject obj = Instantiate(snowballPrefab, spawnPos, Quaternion.identity);
            SnowBall ball = obj.GetComponent<SnowBall>();

            if (ball != null)
            {
                snowballAlive = true;
                ball.Init(player, terrain);
            }
            else
            {
                Debug.LogError("❌ Prefab thiếu SnowBall component!");
                Destroy(obj);
            }
        }
    }

    public void OnSnowballPopped()
    {
        snowballAlive = false;
    }

    public void ResetSnowCounter()
    {
        snowballAlive = false;
        heavyAllCleared = false;
        snowballPopped = false;
        heavyRemaining = heavyPixelsInitial;
        totalRemaining = totalPixelsInitial;
    }

    bool IsInZone(Vector3 worldPos, Collider zone)
    {
        if (zone == null) return false;
        return zone.bounds.Contains(worldPos);
    }
}