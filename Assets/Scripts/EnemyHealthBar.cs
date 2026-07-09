using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider slider;

    private EnemyController _enemy;

    private void Start()
    {
        _enemy = GetComponentInParent<EnemyController>();
        if (_enemy == null) return;

        slider.maxValue = _enemy.GetMaxHealth();
        slider.value = _enemy.GetCurrentHealth();
        _enemy.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        if (_enemy != null)
            _enemy.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
