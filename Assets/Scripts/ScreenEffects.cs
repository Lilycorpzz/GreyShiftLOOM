using UnityEngine;
using UnityEngine.UI;

public class ScreenEffects : MonoBehaviour
{
    [Header("Overlay (UI Image)")]
    public Image darkOverlay;         // assign your DarkOverlay UI Image here
    [Tooltip("Maximum dark alpha when spark = 0")]
    [Range(0f, 1f)] public float maxAlpha = 0.85f;

    private void Start()
    {
        if (darkOverlay == null) Debug.LogError("Assign the DarkOverlay Image in inspector.");
        if (SparkManager.Instance != null)
        {
            SparkManager.Instance.OnSparkChanged += OnSparkChanged;
            // initialize overlay
            OnSparkChanged(SparkManager.Instance.GetNormalizedSpark());
        }
    }

    private void OnDestroy()
    {
        if (SparkManager.Instance != null)
            SparkManager.Instance.OnSparkChanged -= OnSparkChanged;
    }

    private void OnSparkChanged(float normalizedSpark)
    {
        // normalizedSpark: 1 = full spark (clear), 0 = no spark (max dark)
        float alpha = Mathf.Lerp(maxAlpha, 0f, normalizedSpark);
        if (darkOverlay != null)
        {
            Color c = darkOverlay.color;
            c.a = alpha;
            darkOverlay.color = c;
        }
    }
}
