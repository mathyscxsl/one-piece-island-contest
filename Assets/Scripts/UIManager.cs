using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game Over Timing")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayBeforeGameOver = 1f;

    private Slider _healthSlider;
    private CanvasGroup _blackPanel;
    private GameObject _gameOverPanel;
    private PlayerController _player;

    private TMP_Text _waveText;
    private TMP_Text _enemiesText;
    private WaveManager _waveManager;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void RegisterWaveUI(TMP_Text waveText, TMP_Text enemiesText)
    {
        _waveText = waveText;
        _enemiesText = enemiesText;

        if (_waveText != null) _waveText.text = "Vague : 1";
        if (_enemiesText != null) _enemiesText.text = "Ennemis restants : 0";
    }

    public void RegisterUI(Slider healthSlider, CanvasGroup blackPanel, GameObject gameOverPanel)
    {
        _healthSlider = healthSlider;
        _blackPanel = blackPanel;
        _gameOverPanel = gameOverPanel;

        _blackPanel.alpha = 0f;
        _gameOverPanel.SetActive(false);

        if (_player != null)
            UpdateHealthBar(_player.GetCurrentHealth(), _player.GetMaxHealth());
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (_player != null)
            _player.OnHealthChanged -= UpdateHealthBar;

        if (_waveManager != null)
        {
            _waveManager.OnWaveStarted -= UpdateWaveText;
            _waveManager.OnEnemiesAliveChanged -= UpdateEnemiesText;
        }

        GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        GameManager.Instance.OnStateChanged += OnGameStateChanged;

        _player = FindAnyObjectByType<PlayerController>();
        if (_player != null)
            _player.OnHealthChanged += UpdateHealthBar;

        _waveManager = FindAnyObjectByType<WaveManager>();
        if (_waveManager != null)
        {
            _waveManager.OnWaveStarted += UpdateWaveText;
            _waveManager.OnEnemiesAliveChanged += UpdateEnemiesText;
        }
    }

    private void UpdateWaveText(int wave)
    {
        if (_waveText != null)
            _waveText.text = $"Vague : {wave}";
    }

    private void UpdateEnemiesText(int count)
    {
        if (_enemiesText != null)
            _enemiesText.text = $"Ennemis restants : {count}";
    }

    private void UpdateHealthBar(int current, int max)
    {
        if (_healthSlider == null) return;
        _healthSlider.maxValue = max;
        _healthSlider.value = current;
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.GameOver)
            StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _blackPanel.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        _blackPanel.alpha = 1f;

        yield return new WaitForSeconds(delayBeforeGameOver);
        _gameOverPanel.SetActive(true);
    }

    public void OnReplay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
