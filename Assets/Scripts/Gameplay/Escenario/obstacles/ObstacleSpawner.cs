using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private GameObject speedBoostPrefab;

    [Header("Platform reference")]
    [SerializeField] private Transform[] platformPieces;

    [Header("Spawn settings")]
    [SerializeField] private int obstacleCount = 6;
    [SerializeField] private int speedBoostCount = 2;
    [SerializeField] private float minSpacingZ = 4f;
    [SerializeField] private float marginX = 1f;
    [SerializeField] private float marginZ = 3f;

    [Header("Seed")]
    [SerializeField] private int seed = 0;
    [SerializeField] private bool randomizeSeedOnStart = true;

    public int CurrentSeed => seed;

    private void Start()
    {
        if (randomizeSeedOnStart)
            seed = Random.Range(0, 999999);

        Spawn(seed);
    }

    public void Spawn(int spawnSeed)
    {
        seed = spawnSeed;
        Random.InitState(seed);

        if (platformPieces == null || platformPieces.Length == 0) return;

        List<Vector3> usedPositions = new List<Vector3>();

        foreach (Transform piece in platformPieces)
        {
            if (piece == null) continue;
            if (piece.GetComponent<Collider>() == null) continue;

            SpawnGroup(obstaclePrefabs, obstacleCount, piece, usedPositions);
            SpawnGroup(new[] { speedBoostPrefab }, speedBoostCount, piece, usedPositions);
        }
    }

    private void SpawnGroup(GameObject[] prefabs, int count, Transform piece, List<Vector3> used)
    {
        if (prefabs == null || prefabs.Length == 0) return;

        Vector3 scale = piece.localScale;
        float marginLX = marginX / scale.x;
        float marginLZ = marginZ / scale.z;
        float minLX = -0.5f + marginLX;
        float maxLX = 0.5f - marginLX;
        float minLZ = -0.5f + marginLZ;
        float maxLZ = 0.5f - marginLZ;
        Vector3 normal = piece.up;

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            if (prefab == null) continue;

            Vector3 spawnPos = GetSpawnPosition(piece, minLX, maxLX, minLZ, maxLZ, used);
            if (spawnPos == Vector3.positiveInfinity) continue;

            used.Add(spawnPos);

            GameObject obj = Instantiate(prefab, spawnPos, piece.rotation);

            Collider objCol = obj.GetComponent<Collider>();
            float halfH = objCol != null
                ? objCol.bounds.extents.y
                : obj.transform.localScale.y * 0.5f;

            obj.transform.position = spawnPos + normal * halfH;
        }
    }

    private Vector3 GetSpawnPosition(Transform piece,
                                     float minLX, float maxLX,
                                     float minLZ, float maxLZ,
                                     List<Vector3> used, int maxTries = 30)
    {
        for (int t = 0; t < maxTries; t++)
        {
            float lx = Random.Range(minLX, maxLX);
            float lz = Random.Range(minLZ, maxLZ);
            Vector3 candidate = piece.TransformPoint(new Vector3(lx, 0.5f, lz));

            bool valid = true;
            foreach (Vector3 pos in used)
                if (Vector3.Distance(candidate, pos) < minSpacingZ) { valid = false; break; }

            if (valid) return candidate;
        }
        return Vector3.positiveInfinity;
    }
}