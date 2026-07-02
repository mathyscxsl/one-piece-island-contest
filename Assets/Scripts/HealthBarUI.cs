using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private PlayerController player;

    private void Update()
    {
        if (player == null) return;

        healthSlider.maxValue = player.GetMaxHealth();
        healthSlider.value = player.GetCurrentHealth();
    }
}