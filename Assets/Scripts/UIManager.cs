using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Health Bar")]
    [SerializeField] private Slider healthSlider;

    [Header("Game Over")]
    [SerializeField] private CanvasGroup blackPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayBeforeGameOver = 1f;

    private PlayerController _player;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        blackPanel.alpha = 0f;
        gameOverPanel.SetActive(false);

        _player = FindAnyObjectByType<PlayerController>();
        if (_player != null)
        {
            _player.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(_player.GetCurrentHealth(), _player.GetMaxHealth());
        }

        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (_player != null)
            _player.OnHealthChanged -= UpdateHealthBar;

        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void UpdateHealthBar(int current, int max)
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = max;
        healthSlider.value = current;
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
            blackPanel.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        blackPanel.alpha = 1f;

        yield return new WaitForSeconds(delayBeforeGameOver);
        gameOverPanel.SetActive(true);
    }

    public void OnReplay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
