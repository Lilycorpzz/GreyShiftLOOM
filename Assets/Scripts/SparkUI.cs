using UnityEngine;
using UnityEngine.UI;

public class SparkUI : MonoBehaviour
{
    public Slider sparkSlider; // assign SparkSlider in inspector

    private void Start()
    {
        if (sparkSlider == null) Debug.LogError("Assign SparkSlider in inspector.");

        if (SparkManager.Instance != null)
        {
            SparkManager.Instance.OnSparkChanged += OnSparkChanged;
            OnSparkChanged(SparkManager.Instance.GetNormalizedSpark());
        }
    }

    private void OnDestroy()
    {
        if (SparkManager.Instance != null)
            SparkManager.Instance.OnSparkChanged -= OnSparkChanged;
    }

    private void OnSparkChanged(float normalized)
    {
        if (sparkSlider != null)
            sparkSlider.value = normalized;
    }
}
