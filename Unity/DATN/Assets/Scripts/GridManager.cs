using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float cellSize = 2.5f;
    public static GridManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public Vector3 SnapToGrid(Vector3 pos)
    {
        float originX = 312.5f;
        float originZ = 260f;

        float x = Mathf.Round((pos.x - originX) / cellSize) * cellSize + originX;
        float z = Mathf.Round((pos.z - originZ) / cellSize) * cellSize + originZ;
        return new Vector3(x, pos.y, z);
    }
}
