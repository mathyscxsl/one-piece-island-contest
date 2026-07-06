using UnityEngine;
using UnityEngine.UI;

public class UIRoot : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private CanvasGroup blackPanel;
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        UIManager.Instance?.RegisterUI(healthSlider, blackPanel, gameOverPanel);
    }
}
