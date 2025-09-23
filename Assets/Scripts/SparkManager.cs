using System;
using UnityEngine;
using System.Collections;

public class SparkManager : MonoBehaviour
{
    public static SparkManager Instance { get; private set; }

    [Header("Spark Settings")]
    public float maxSpark = 100f;
    [Tooltip("Starting spark value.")]
    public float startingSpark = 100f;
    public float currentSpark;

    // raised with normalized value (0..1)
    public Action<float> OnSparkChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        currentSpark = Mathf.Clamp(startingSpark, 0, maxSpark);
    }

    private void Start()
    {
        // broadcast initial value
        OnSparkChanged?.Invoke(GetNormalizedSpark());
    }

    /// <summary>
    /// Change spark immediately (positive or negative). Clamps and notifies listeners.
    /// </summary>
    public void ChangeSparkImmediate(float amount)
    {
        currentSpark = Mathf.Clamp(currentSpark + amount, 0f, maxSpark);
        OnSparkChanged?.Invoke(GetNormalizedSpark());
    }

    /// <summary>
    /// Drain (or add) spark gradually over duration. Positive totalDrain reduces spark when totalDrain &gt; 0.
    /// Call with StartCoroutine(SparkManager.Instance.DrainSparkOverTime(drainAmount, duration));
    /// </summary>
    public IEnumerator DrainSparkOverTime(float totalAmount, float duration)
    {
        if (duration <= 0f)
        {
            ChangeSparkImmediate(-totalAmount);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float delta = (totalAmount * Time.deltaTime) / duration;
            ChangeSparkImmediate(-delta);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public float GetNormalizedSpark()
    {
        if (maxSpark <= 0.0001f) return 0f;
        return Mathf.Clamp01(currentSpark / maxSpark);
    }

    // debug helper
    [ContextMenu("Fill Spark")]
    public void FillSpark() => ChangeSparkImmediate(maxSpark - currentSpark);

}
