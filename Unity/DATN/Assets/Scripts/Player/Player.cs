using System.Collections.Generic;
using UnityEngine;

public class GridPlayer : MonoBehaviour
{
    public static GridPlayer Instance;

    [Header("Movement")]
    public float moveSpeed = 15f;

    [Header("State")]
    public bool hasShovel = false;
    public bool hasFinalKey = false;

    [Header("Entities có thể là YOU")]
    public GameObject snowmanObject;
    public GameObject shovelObject;
    public List<GameObject> fenceObjects;

    [Header("Boundary")]
    public Collider boundaryCollider;
    public Collider[] forbiddenZones;

    [Header("Word Board")]
    public Collider wordBoardCollider;
    public float stepSize = 2.5f;

    [Header("Animation")]
    public Animator snowmanAnimator;

    Rigidbody rb;
    int terrainLayer;
    Vector3 moveDir = Vector3.zero;
    Vector3 lastSafePos;
    GridManager grid;

    // Grid step state
    bool keyHeld = false;
    float lastStepTime = -1f;
    float stepCooldown = 0.2f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        rb = GetComponent<Rigidbody>();
        terrainLayer = LayerMask.GetMask("Terrain");
        lastSafePos = transform.position;
        grid = FindObjectOfType<GridManager>();

        if (snowmanAnimator == null)
            snowmanAnimator = GetComponentInChildren<Animator>();
    }

    bool wasInBoard = false;

    void Start()
    {
        // Snap Snowman về grid ngay từ đầu
        Vector3 snapped = grid.SnapToGrid(transform.position);
        transform.position = new Vector3(snapped.x, transform.position.y, snapped.z);
    }

    void Update()
    {
        ReadInput();

        bool inBoard = IsInWordBoard();
       
        // Snap về grid ngay khi bước vào bàn cờ
        if (inBoard && !wasInBoard)
        {
            Vector3 snapped = grid.SnapToGrid(transform.position);
            transform.position = new Vector3(snapped.x, transform.position.y, snapped.z);
        }
        wasInBoard = inBoard;

        if (inBoard)
            MoveGridStep();
        else
            MoveSmooth();

        AlignToTerrain();
        ClampToBoundary();
        TrackSnow();
        UpdateAnimation();
    }

    void ReadInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(-h, 0, -v).normalized;
    }

    bool IsInWordBoard()
    {
        if (wordBoardCollider == null) return false;
        return wordBoardCollider.bounds.Contains(transform.position);
    }

    // ── TRONG BÀN CỜ: di chuyển theo từng ô ──
    void MoveGridStep()
    {
        if (moveDir == Vector3.zero) { keyHeld = false; return; }
        if (keyHeld) return;
        if (Time.time - lastStepTime < stepCooldown) return;

        keyHeld = true;
        lastStepTime = Time.time;

        Vector3 snappedPos = grid.SnapToGrid(transform.position);

        Vector3 step;
        if (Mathf.Abs(moveDir.x) >= Mathf.Abs(moveDir.z))
            step = new Vector3(Mathf.Sign(moveDir.x) * stepSize, 0, 0);
        else
            step = new Vector3(0, 0, Mathf.Sign(moveDir.z) * stepSize);

        Vector3 checkDir = step.normalized;
        Ray ray = new Ray(snappedPos + Vector3.up * 0.5f, checkDir);

        if (Physics.Raycast(ray, out RaycastHit hit, stepSize * 0.8f))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name + " Tag: " + hit.collider.tag);
            WordTile tile = hit.collider.GetComponentInParent<WordTile>();
            if (tile != null)
            {
                bool pushed = tile.TryPush(checkDir);
                foreach (WordTile t in FindObjectsOfType<WordTile>())
                    t.ApplyPendingMove();
                if (!pushed) return;
            }

            Ray fenceRay = new Ray(snappedPos + Vector3.up * 0.5f, checkDir);
            if (Physics.Raycast(fenceRay, out RaycastHit fenceHit, stepSize * 0.8f))
            {
                if (fenceHit.collider != null &&
                    fenceHit.collider.CompareTag("Fence") &&
                    RuleManager.Instance.IsStop("Fence")) return;

                if (fenceHit.collider != null &&
                    fenceHit.collider.CompareTag("Gate") &&
                    !hasFinalKey) return;
            }
        }

        Vector3 nextPos = snappedPos + step;
        rb.MovePosition(nextPos);
        MoveAllYouGridStep(step, checkDir);
        transform.rotation = Quaternion.LookRotation(checkDir);
        SoundManager.Instance?.PlayFootstep();
    }

    void MoveAllYouGridStep(Vector3 step, Vector3 checkDir)
    {
        if (RuleManager.Instance.IsYou("Shovel") && shovelObject != null)
        {
            Vector3 shovelSnapped = grid.SnapToGrid(shovelObject.transform.position);
            Ray ray = new Ray(shovelSnapped + Vector3.up * 0.5f, checkDir);
            if (Physics.Raycast(ray, out RaycastHit hit, stepSize * 0.8f))
            {
                WordTile tile = hit.collider.GetComponentInParent<WordTile>();
                if (tile != null) tile.TryPush(checkDir);

                // ← thêm check Fence
                if (hit.collider.CompareTag("Fence") && RuleManager.Instance.IsStop("Fence"))
                    return;
            }
            shovelObject.transform.position = shovelSnapped + step;
        }
    }

    // ── NGOÀI BÀN CỜ: di chuyển mượt ──
    void MoveSmooth()
    {
        if (moveDir == Vector3.zero) return;
        if (RuleManager.Instance == null) return;

        if (RuleManager.Instance.IsYou("Snowman"))
            MoveSmoothEntity(transform, moveDir, isPlayer: true);

        if (RuleManager.Instance.IsYou("Shovel") && shovelObject != null)
            MoveSmoothEntity(shovelObject.transform, moveDir, isPlayer: false);

        if (RuleManager.Instance.IsYou("Fence"))
        {
            foreach (GameObject fence in fenceObjects)
            {
                if (fence != null)
                    MoveSmoothEntity(fence.transform, moveDir, isPlayer: false);
            }
        }
    }

    void MoveSmoothEntity(Transform entity, Vector3 dir, bool isPlayer)
    {
        Vector3 pushDir = new Vector3(0, 0, dir.z).normalized;
        if (pushDir != Vector3.zero)
        {
            Ray ray = new Ray(entity.position + Vector3.up * 0.5f, pushDir);
            if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
            {
                if (hit.collider.CompareTag("Fence") && RuleManager.Instance.IsStop("Fence"))
                    return;
                if (hit.collider.CompareTag("Gate") && !hasFinalKey)
                    return;
            }
        }

        Vector3 sideDir = new Vector3(dir.x, 0, 0).normalized;
        if (sideDir != Vector3.zero)
        {
            Ray sideRay = new Ray(entity.position + Vector3.up * 0.5f, sideDir);
            if (Physics.Raycast(sideRay, out RaycastHit sideHit, 1.5f))
            {
                if (sideHit.collider.CompareTag("Fence") && RuleManager.Instance.IsStop("Fence"))
                    dir = new Vector3(0, 0, dir.z);
                if (sideHit.collider.CompareTag("Gate") && !hasFinalKey)
                    dir = new Vector3(0, 0, dir.z);
            }
        }

        if (dir == Vector3.zero) return;

        if (isPlayer)
        {
            Vector3 nextPos = entity.position + dir * moveSpeed * Time.deltaTime;
            rb.MovePosition(nextPos);
            entity.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            entity.position += dir * moveSpeed * Time.deltaTime;
        }
    }

    void TrackSnow()
    {
        if (SnowManager.Instance == null) return;
        bool isMoving = moveDir.magnitude > 0.1f;

        if (RuleManager.Instance.IsYou("Snowman"))
            SnowManager.Instance.TrackPosition(transform.position, isMoving);

        if (RuleManager.Instance.IsYou("Shovel")
            && RuleManager.Instance.IsClear("Shovel")
            && shovelObject != null)
        {
            SnowManager.Instance.TrackPosition(shovelObject.transform.position, isMoving);
        }
    }

    void UpdateAnimation()
    {
        if (snowmanAnimator == null) return;
        snowmanAnimator.SetFloat("Speed", moveDir.magnitude);
    }

    void AlignToTerrain()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 10f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, terrainLayer))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    void ClampToBoundary()
    {
        if (boundaryCollider == null) return;

        Bounds b = boundaryCollider.bounds;
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, b.min.x, b.max.x);
        pos.z = Mathf.Clamp(pos.z, b.min.z, b.max.z);

        foreach (Collider zone in forbiddenZones)
        {
            if (zone != null && zone.bounds.Contains(pos))
            {
                pos = lastSafePos;
                break;
            }
        }

        lastSafePos = pos;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            hasFinalKey = true;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Shovel"))
        {
            if (!RuleManager.Instance.IsYou("Shovel"))
            {
                hasShovel = true;
                Destroy(other.gameObject);
            }
        }
    }
}