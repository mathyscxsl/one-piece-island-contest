using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup blackPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayBeforeGameOver = 1f;

    private void Start()
    {
        blackPanel.alpha = 0f;
        gameOverPanel.SetActive(false);
    }

    public void Show()
    {
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

        Debug.Log($"Activation GameOverPanel, gameOverPanel = {gameOverPanel}");
        gameOverPanel.SetActive(true);
    }

    public void OnReplay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}