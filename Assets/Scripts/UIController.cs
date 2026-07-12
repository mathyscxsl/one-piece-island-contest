using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private Slider healthSlider;

    [Header("Wave Info")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemiesText;

    [Header("Game Over")]
    [SerializeField] private CanvasGroup blackPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text wavesSurvivedText;

    private void Start()
    {
        UIManager.Instance?.RegisterUI(healthSlider, blackPanel, gameOverPanel);
        UIManager.Instance?.RegisterWaveUI(waveText, enemiesText);
        UIManager.Instance?.RegisterWavesSurvivedText(wavesSurvivedText);
    }

    public void OnReplay()
    {
        UIManager.Instance?.OnReplay();
    }
}
