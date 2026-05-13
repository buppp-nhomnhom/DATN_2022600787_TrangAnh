using UnityEngine;

public class WordTile : MonoBehaviour
{
    public string word;

    private Vector3 pendingPosition;
    private bool pushedThisFrame = false;

    GridManager grid;

    void Awake()
    {
        grid = FindObjectOfType<GridManager>();
        pendingPosition = transform.position;
    }

    void Update()
    {
        pushedThisFrame = false;
        pendingPosition = transform.position;
    }

    public bool TryPush(Vector3 dir)
    {
        if (pushedThisFrame) return true;

        Vector3 next = grid.SnapToGrid(pendingPosition + dir * grid.cellSize);

        // Check boundary bàn cờ
        Collider boardCollider = GridPlayer.Instance?.wordBoardCollider;
        if (boardCollider != null && !boardCollider.bounds.Contains(next))
            return false;

        WordTile other = RuleManager.Instance?.GetTileAtPending(next);
        if (other != null && !other.TryPush(dir)) return false;

        pushedThisFrame = true;
        pendingPosition = next;
        return true;
    }

    public void ApplyPendingMove()
    {
        if (pushedThisFrame)
        {
            transform.position = pendingPosition;
            SoundManager.Instance?.PlayPush();
        }
    }

    public Vector3 GetPendingPosition() => pendingPosition;
}