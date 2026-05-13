using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : MonoBehaviour
{
    public static RuleManager Instance;

    GridManager grid;

    public HashSet<string> youSet = new HashSet<string>();
    public HashSet<string> stopSet = new HashSet<string>();
    public HashSet<string> openSet = new HashSet<string>();
    public HashSet<string> clearSet = new HashSet<string>();

    void Awake()
    {
        Instance = this;
        grid = FindObjectOfType<GridManager>();
    }

    void Update()
    {
        ParseRules();
        ApplyRules();
    }

    void ParseRules()
    {
        youSet.Clear();
        stopSet.Clear();
        openSet.Clear();
        clearSet.Clear();

        // KHÔNG default Snowman is You nữa
        // Phải ghép chữ mới có effect

        foreach (WordTile tile in FindObjectsOfType<WordTile>())
        {
            if (tile.word != "Is") continue;

            Vector3 pos = grid.SnapToGrid(tile.transform.position);

            // Scan ngang
            CheckRule(
                GetTileAt(pos + Vector3.right * grid.cellSize),
                GetTileAt(pos + Vector3.left * grid.cellSize));

            // Scan dọc
            CheckRule(
                GetTileAt(pos + Vector3.back * grid.cellSize),
                GetTileAt(pos + Vector3.forward * grid.cellSize));
        }
    }

    public WordTile GetTileAtPending(Vector3 pos)
    {
        foreach (WordTile t in FindObjectsOfType<WordTile>())
            if (grid.SnapToGrid(t.GetPendingPosition()) == grid.SnapToGrid(pos))
                return t;
        return null;
    }

    void CheckRule(WordTile noun, WordTile prop)
    {
        if (noun == null || prop == null) return;
        if (!IsNoun(noun.word) || !IsProp(prop.word)) return;

        switch (prop.word)
        {
            case "You": youSet.Add(noun.word); break;
            case "Stop": stopSet.Add(noun.word); break;
            case "Open": openSet.Add(noun.word); break;
            case "Clear": clearSet.Add(noun.word); break;
        }
    }

    void ApplyRules()
    {
        ApplyFenceStop();
        ApplyShovelPickup();
    }

    void ApplyFenceStop()
    {
        GameObject[] fences = GameObject.FindGameObjectsWithTag("Fence");
        foreach (GameObject fence in fences)
        {
            Collider col = fence.GetComponent<Collider>();
            if (col != null)
                col.enabled = stopSet.Contains("Fence");
        }
    }

    void ApplyShovelPickup()
    {
        // Shovel is You → Shovel không nhặt được (tắt trigger)
        // Shovel is You off → Shovel nhặt được (bật trigger)
        GameObject shovel = GameObject.FindWithTag("Shovel");
        if (shovel == null) return;

        Collider col = shovel.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = !youSet.Contains("Shovel");
    }

    // Helpers
    public WordTile GetTileAt(Vector3 pos)
    {
        foreach (WordTile t in FindObjectsOfType<WordTile>())
            if (grid.SnapToGrid(t.transform.position) == grid.SnapToGrid(pos))
                return t;
        return null;
    }

    bool IsNoun(string w) => w == "Snowman" || w == "Fence"
                          || w == "Shovel" || w == "Key";

    bool IsProp(string w) => w == "You" || w == "Stop"
                          || w == "Open" || w == "Clear";

    public bool IsYou(string n) => youSet.Contains(n);
    public bool IsStop(string n) => stopSet.Contains(n);
    public bool IsOpen(string n) => openSet.Contains(n);
    public bool IsClear(string n) => clearSet.Contains(n);
}