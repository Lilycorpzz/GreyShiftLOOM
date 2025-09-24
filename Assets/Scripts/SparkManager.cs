using System;
using UnityEngine;
using System.Collections;

public class SparkManager : MonoBehaviour
{
    public static SparkManager Instance { get; private set; }

    [Header("Spark Settings")]
    public float maxSpark = 100f;
    public float startingSpark = 100f;
    public float currentSpark;

    // normalized value (0..1) subscribers use this to update overlays / UI
    public Action<float> OnSparkChanged;

    // fired once when spark reaches zero
    public Action OnSparkDepleted;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        currentSpark = Mathf.Clamp(startingSpark, 0f, maxSpark);
    }

    private void Start()
    {
        OnSparkChanged?.Invoke(GetNormalizedSpark());
    }

    public void ChangeSparkImmediate(float amount)
    {
        // positive amount adds spark, negative amount subtracts
        currentSpark = Mathf.Clamp(currentSpark + amount, 0f, maxSpark);
        OnSparkChanged?.Invoke(GetNormalizedSpark());
        if (Mathf.Approximately(currentSpark, 0f))
            OnSparkDepleted?.Invoke();
    }

    public IEnumerator DrainSparkOverTime(float totalAmount, float duration)
    {
        // totalAmount expected positive (we subtract)
        if (duration <= 0f)
        {
            ChangeSparkImmediate(-totalAmount);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float step = (totalAmount * Time.deltaTime) / duration;
            ChangeSparkImmediate(-step);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator RestoreSparkOverTime(float totalAmount, float duration)
    {
        // restore spark over duration
        if (duration <= 0f)
        {
            ChangeSparkImmediate(totalAmount);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float step = (totalAmount * Time.deltaTime) / duration;
            ChangeSparkImmediate(step);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public float GetNormalizedSpark()
    {
        if (maxSpark <= 0.0001f) return 0f;
        return Mathf.Clamp01(currentSpark / maxSpark);
    }

    [ContextMenu("FillSpark")]
    public void FillSpark() => ChangeSparkImmediate(maxSpark - currentSpark);

}
