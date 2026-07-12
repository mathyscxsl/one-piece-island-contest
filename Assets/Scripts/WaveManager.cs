using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class WaveManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private float waveDelay = 5f;

    [Header("Wave Settings")]
    [SerializeField] private int baseEnemyCount = 3;
    [SerializeField] private int enemiesPerWave = 2;

    [Header("Scaling every 5 waves")]
    [SerializeField] private float healthMultiplier = 1.5f;
    [SerializeField] private float damageMultiplier = 1.3f;

    public event Action<int> OnWaveStarted;
    public event Action<int> OnEnemiesAliveChanged;

    private int _currentWave = 0;
    private int _enemiesAlive = 0;
    private bool _waveSpawning = false;
    private bool _gameOver = false;


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();

        _currentWave = 0;
        _enemiesAlive = 0;
        _waveSpawning = false;
        _gameOver = false;

        RefreshSpawnPoints();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
        }

        StartCoroutine(StartNextWave());
    }

    private void RefreshSpawnPoints()
    {
        GameObject parent = GameObject.Find("SpawnPoints");
        if (parent == null) return;

        int count = parent.transform.childCount;
        spawnPoints = new Transform[count];
        for (int i = 0; i < count; i++)
            spawnPoints[i] = parent.transform.GetChild(i);
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.GameOver)
            _gameOver = true;
    }


    private IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(waveDelay);
        if (_gameOver) yield break;

        _currentWave++;
        OnWaveStarted?.Invoke(_currentWave);
        _waveSpawning = true;

        int count = baseEnemyCount + (_currentWave - 1) * enemiesPerWave;
        int tier = (_currentWave - 1) / 5;
        float hpMult = Mathf.Pow(healthMultiplier, tier);
        float dmgMult = Mathf.Pow(damageMultiplier, tier);

        for (int i = 0; i < count; i++)
        {
            if (_gameOver) yield break;
            SpawnEnemy(hpMult, dmgMult);
            yield return new WaitForSeconds(spawnInterval);
        }

        _waveSpawning = false;

        if (_enemiesAlive <= 0 && !_gameOver)
            StartCoroutine(StartNextWave());
    }


    private void SpawnEnemy(float hpMult, float dmgMult)
    {
        if (enemyPrefab == null || spawnPoints.Length == 0) return;

        Transform point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject obj = Instantiate(enemyPrefab, point.position, Quaternion.identity);

        EnemyController enemy = obj.GetComponent<EnemyController>();
        if (enemy == null) return;

        enemy.Configure(hpMult, dmgMult);
        enemy.OnDeath += OnEnemyDied;
        _enemiesAlive++;
        OnEnemiesAliveChanged?.Invoke(_enemiesAlive);
    }


    private void OnEnemyDied()
    {
        _enemiesAlive--;
        OnEnemiesAliveChanged?.Invoke(_enemiesAlive);
        if (_enemiesAlive <= 0 && !_waveSpawning && !_gameOver)
            StartCoroutine(StartNextWave());
    }
}
