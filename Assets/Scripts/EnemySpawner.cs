using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float initialDelay = 1f;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        if (IslandManager.Instance == null || IslandManager.Instance.CurrentIsland == null) yield break;

        IslandData island = IslandManager.Instance.CurrentIsland;

        if (island.monsterPrefab == null || spawnPoints.Length == 0) yield break;

        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < island.monsterCount; i++)
        {
            Transform point = spawnPoints[i % spawnPoints.Length];
            Instantiate(island.monsterPrefab, point.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
