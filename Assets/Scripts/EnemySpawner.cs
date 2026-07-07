using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (IslandManager.Instance == null || IslandManager.Instance.CurrentIsland == null) return;

        IslandData island = IslandManager.Instance.CurrentIsland;

        if (island.monsterPrefab == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < island.monsterCount; i++)
        {
            Transform point = spawnPoints[i % spawnPoints.Length];
            Instantiate(island.monsterPrefab, point.position, Quaternion.identity);
        }
    }
}
