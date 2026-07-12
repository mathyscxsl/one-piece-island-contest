using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private CanvasGroup blackPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Wave Info")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemiesText;

    private void Start()
    {
        UIManager.Instance?.RegisterUI(healthSlider, blackPanel, gameOverPanel);
        UIManager.Instance?.RegisterWaveUI(waveText, enemiesText);
    }

    public void OnReplay()
    {
        UIManager.Instance?.OnReplay();
    }
}
