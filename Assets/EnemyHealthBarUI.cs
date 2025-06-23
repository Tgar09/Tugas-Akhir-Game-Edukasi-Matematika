using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private Transform target;

    public void Initialize(Transform enemyTransform)
    {
        target = enemyTransform;
    }

    public void SetHealth(float current, float max)
    {
        slider.value = current / max;
    }
}
