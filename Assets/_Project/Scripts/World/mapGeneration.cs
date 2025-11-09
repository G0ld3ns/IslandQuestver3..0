using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    [Header("Grid")]
    public GridGizmo gridGizmo;
    [Header("Prefabs")]
    public GameObject[] wallPrefabs;

    [Header("Origin")]
    [SerializeField] bool originIsCenter = false;
    [Header("SpawnProtection")]
    public Vector2Int spawnSafe = new Vector2Int(2, 2);
    [Header("Wall Box")]
    public int boxSize = 3;            
    [Range(0, 1)] public float boxSpawnChance = 0.8f;
    public int minTilesInBox = 3;      
    public int maxTilesInBox = 9;   
    public int separation = 1;

    [Header("Chest")]
    public GameObject chestPrefab;
    public int chestBlockSize = 3;
    Vector2Int chestCenterCell;

    [Header("Prefabs Height")]
    public float yOffSet = 0.05f;
    public bool generateOnPlay = true;

    [Header("Gameplay prefabs")]
    public GameObject strongDamageCamp;
    public GameObject weakDamageCamp; 
    public GameObject healPrefab;
    public GameObject trap1Prefab;
    public GameObject trap2Prefab;
    public int trapCount = 3;
    public GameObject[] enemyPrefabs; 
    public int enemyCount = 3;  

    private Vector3 gridCorner;
    private Vector2Int gridSize;
    private float gridCellSize;

    bool[,] occupied;
    bool[,] blocked;
    void Start()
    {
        if (generateOnPlay)
            GenerateMap();
    }

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        if (gridGizmo == null)
        {
            Debug.LogError("Grid not found!");
            return;
        }

        gridSize = gridGizmo.size;
        gridCellSize = gridGizmo.cellSize;
        gridCorner = GetGridOrigin();

        occupied = new bool[gridSize.x, gridSize.y];
        blocked = new bool[gridSize.x, gridSize.y];

        for (int x = 0; x < Mathf.Min(spawnSafe.x, gridSize.x); x++)
            for (int y = 0; y < Mathf.Min(spawnSafe.y, gridSize.y); y++)
                blocked[x, y] = true;

        ClearPreviousObjects();
        ReserveTopBandChestBlock();
        for (int ax = 0; ax <= gridSize.x - boxSize; ax++)
        {
            for (int ay = 0; ay <= gridSize.y - boxSize; ay++)
            {

                if (Random.value > boxSpawnChance) continue;

                if (RegionOverlapsBlocked(ax, ay, boxSize, separation)) continue;

                int need = Mathf.Clamp(Random.Range(minTilesInBox, maxTilesInBox + 1), 0, boxSize * boxSize);
                
                var cells = new List<Vector2Int>(boxSize * boxSize);
                for (int dx = 0; dx < boxSize; dx++)
                    for (int dy = 0; dy < boxSize; dy++)
                        cells.Add(new Vector2Int(ax + dx, ay + dy));

                FisherYatesShuffle(cells);

                for (int i = 0; i < need; i++)
                {
                    var c = cells[i];
                    occupied[c.x, c.y] = true;
                }

                MarkBlockedWithMargin(ax, ay, boxSize, separation);
            }
        }

        BuildWalls();
        PlaceChest();
        SpawnDamageCamp();
        SpawnHeal();
        SpawnTraps();
        SpawnEnemies();

        Debug.Log("Map generated");

    }

    void SpawnDamageCamp()
    {
        if (!strongDamageCamp && !weakDamageCamp) return;

        GameObject prefabToUse = null;

        if (strongDamageCamp && weakDamageCamp)
        {
            prefabToUse = (Random.value < 0.5f) ? strongDamageCamp : weakDamageCamp;
        }
        else if (strongDamageCamp)
        {
            prefabToUse = strongDamageCamp;
        }
        else if (weakDamageCamp)
        {
            prefabToUse = weakDamageCamp;
        }

        if (!prefabToUse) return;

        Vector2Int? maybeCell = GetRandomFreeCell();
        if (maybeCell == null) return;

        Vector2Int cell = maybeCell.Value;

        Vector3 pos = gridCorner + new Vector3(
            (cell.x + 0.5f) * gridCellSize,
            yOffSet,
            (cell.y + 0.5f) * gridCellSize
        );

        Instantiate(prefabToUse, pos, Quaternion.identity, transform);

        occupied[cell.x, cell.y] = true;
    }

    void SpawnHeal()
    {
        if (!healPrefab) return;

        Vector2Int? maybeCell = GetRandomFreeCell();
        if (maybeCell == null) return;

        Vector2Int cell = maybeCell.Value;

        Vector3 pos = gridCorner + new Vector3(
            (cell.x + 0.5f) * gridCellSize,
            yOffSet,
            (cell.y + 0.5f) * gridCellSize
        );

        Instantiate(healPrefab, pos, Quaternion.identity, transform);

        // Pažymim, kad šita cell užimta, kad vėliau čia neatsirastų kiti objektai
        occupied[cell.x, cell.y] = true;
    }

    void SpawnTraps()
    {
        GameObject[] traps = new GameObject[] { trap1Prefab, trap2Prefab };
        int spawned = 0;

        while (spawned < trapCount)
        {
            Vector2Int? maybeCell = GetRandomFreeCell();
            if (maybeCell == null) break;

            var cell = maybeCell.Value;
            var prefab = traps[Random.Range(0, traps.Length)];

            Vector3 pos = gridCorner + new Vector3(
                (cell.x + 0.5f) * gridCellSize,
                yOffSet,
                (cell.y + 0.5f) * gridCellSize
            );

            Instantiate(prefab, pos, Quaternion.identity, transform);
            occupied[cell.x, cell.y] = true;
            spawned++;
        }
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (enemyCount <= 0) return;

        int spawned = 0;

        while (spawned < enemyCount)
        {
            Vector2Int? maybeCell = GetRandomFreeCell();
            if (maybeCell == null) break;

            Vector2Int cell = maybeCell.Value;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector3 pos = gridCorner + new Vector3(
                (cell.x + 0.5f) * gridCellSize,
                yOffSet,
                (cell.y + 0.5f) * gridCellSize
            );

            Instantiate(prefab, pos, Quaternion.identity, transform);
            
            occupied[cell.x, cell.y] = true;
            spawned++;
        }
    }



    bool RegionOverlapsBlocked(int ax, int ay, int bSize, int margin)
    {
        int minX = Mathf.Max(0, ax - margin);
        int minY = Mathf.Max(0, ay - margin);
        int maxX = Mathf.Min(gridSize.x - 1, ax + bSize - 1 + margin);
        int maxY = Mathf.Min(gridSize.y - 1, ay + bSize - 1 + margin);

        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                if (blocked[x, y]) return true;

        return false;
    }

    void MarkBlockedWithMargin(int ax, int ay, int bSize, int margin)
    {
        int minX = Mathf.Max(0, ax - margin);
        int minY = Mathf.Max(0, ay - margin);
        int maxX = Mathf.Min(gridSize.x - 1, ax + bSize - 1 + margin);
        int maxY = Mathf.Min(gridSize.y - 1, ay + bSize - 1 + margin);

        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                blocked[x, y] = true;
    }

    void BuildWalls()
    {
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
            {
                if (!occupied[x, y]) continue;
                if (wallPrefabs == null || wallPrefabs.Length == 0) continue;

                Vector3 pos = gridCorner + new Vector3((x + 0.5f) * gridCellSize, yOffSet, (y + 0.5f) * gridCellSize);
                GameObject prefab = wallPrefabs[Random.Range(0, wallPrefabs.Length)];
                Quaternion rot = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);

                Instantiate(prefab, pos, rot, transform);
            }
    }

    Vector2Int? GetRandomFreeCell()
    {
        var list = new System.Collections.Generic.List<Vector2Int>();

        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
            {
                if (occupied[x, y]) continue;      
                if (x < 2 && y < 2) continue;        

                list.Add(new Vector2Int(x, y));
            }

        if (list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }



    void ReserveTopBandChestBlock()
    {
        int size = Mathf.Max(3, chestBlockSize); 
        size = 3;

        int cy = gridSize.y - 2;
        if (cy < 1) { Debug.LogWarning("Map per žemas chest juostai."); return; }


        int cx = Random.Range(1, Mathf.Max(2, gridSize.x - 1) - 1); 

        for (int x = cx - 1; x <= cx + 1; x++)
        {
            if (x < 0 || x >= gridSize.x) continue;
            for (int y = cy - 1; y <= cy + 1; y++)
            {
                if (y < 0 || y >= gridSize.y) continue;
                blocked[x, y] = true;  
                occupied[x, y] = false; 
            }
        }

        chestCenterCell = new Vector2Int(cx, cy);
    }

    void PlaceChest()
    {
        if (!chestPrefab) return;

        Vector3 pos = gridCorner + new Vector3(
            (chestCenterCell.x + 0.5f) * gridCellSize,
            yOffSet,
            (chestCenterCell.y + 0.5f) * gridCellSize
        );

        Instantiate(chestPrefab, pos, Quaternion.identity, transform);
    }

    void FisherYatesShuffle(List<Vector2Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void ClearPreviousObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    Vector3 GetGridOrigin()
    {
        Vector3 center = gridGizmo.transform.position;

        return new Vector3(
            center.x - gridSize.x * 0.5f * gridCellSize,
            center.y,
            center.z - gridSize.y * 0.5f * gridCellSize
        );
    }
}
