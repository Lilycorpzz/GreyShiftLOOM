using UnityEngine;
using System.Collections;

public class LoomController : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    [Tooltip("How much health the Loom currently has.")]
    public float currentHealth = 100f;

    [Header("Damage Scaling")]
    [Tooltip("Proportion of spark-increase that becomes damage to the Loom. " +
             "e.g. 0.5 means 50% of spark increase (normalized) * maxHealth is applied as damage.")]
    [Range(0f, 1f)] public float damagePerSparkNormalized = 0.25f;

    [Header("Search / Wiggle (no animation)")]
    [Tooltip("When health below this percent (0..1) the Loom does a searching wiggle.")]
    [Range(0f, 1f)] public float searchThresholdNormalized = 0.4f;
    public float wiggleAngle = 6f;          // degrees left/right
    public float wiggleSpeed = 4f;          // wiggles per second
    public float wiggleDuration = 2f;       // seconds per wiggle burst

    [Header("References")]
    public SpriteRenderer sr;               // assign or cached automatically

    // internal
    private float lastNormalizedSpark = -1f;
    private Coroutine wiggleRoutine;
    private bool isFizzlingOut = false;

    private void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    private void Start()
    {
        // initialize last spark
        if (SparkManager.Instance != null)
        {
            lastNormalizedSpark = SparkManager.Instance.GetNormalizedSpark();
            SparkManager.Instance.OnSparkChanged += OnSparkChanged;
        }
    }

    private void OnDestroy()
    {
        if (SparkManager.Instance != null)
            SparkManager.Instance.OnSparkChanged -= OnSparkChanged;
    }

    private void OnSparkChanged(float normalizedSpark)
    {
        // We only damage Loom when spark *increases* (player boosting spark)
        if (normalizedSpark > lastNormalizedSpark)
        {
            float delta = normalizedSpark - lastNormalizedSpark; // 0..1 delta
            float damage = delta * maxHealth * damagePerSparkNormalized;
            ApplyDamage(damage);
        }

        lastNormalizedSpark = normalizedSpark;
    }

    private void ApplyDamage(float amount)
    {
        if (isFizzlingOut) return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        // Optionally update UI for Loom health here
        CheckHealthState();
    }

    private void CheckHealthState()
    {
        float normalized = currentHealth / maxHealth;
        if (normalized <= 0f)
        {
            StartCoroutine(FizzleOutAndDie());
        }
        else if (normalized <= searchThresholdNormalized)
        {
            // start wiggle if not already
            if (wiggleRoutine == null)
                wiggleRoutine = StartCoroutine(WiggleSearchRoutine());
        }
    }

    private IEnumerator WiggleSearchRoutine()
    {
        // Wiggle (look left-right) in bursts while below threshold
        float elapsed = 0f;
        while (currentHealth / maxHealth <= searchThresholdNormalized && !isFizzlingOut)
        {
            elapsed = 0f;
            while (elapsed < wiggleDuration)
            {
                float t = Mathf.Sin(Time.time * Mathf.PI * 2f * wiggleSpeed); // -1..1
                float angle = t * wiggleAngle;
                transform.localRotation = Quaternion.Euler(0f, 0f, angle);
                elapsed += Time.deltaTime;
                yield return null;
            }
            // short pause between wiggle bursts
            yield return new WaitForSeconds(0.3f);
        }

        // restore rotation
        transform.localRotation = Quaternion.identity;
        wiggleRoutine = null;
    }

    private IEnumerator FizzleOutAndDie()
    {
        isFizzlingOut = true;

        // simple visual fizzle: blink and scale down
        float timer = 0f;
        float dur = 1.2f;
        Color start = sr.color;
        Vector3 startScale = transform.localScale;

        while (timer < dur)
        {
            float t = timer / dur;
            // blink alpha
            float alpha = Mathf.PingPong(t * 6f, 1f);
            sr.color = new Color(start.r, start.g, start.b, Mathf.Lerp(1f, 0f, t) * alpha);
            // scale down
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            timer += Time.deltaTime;
            yield return null;
        }

        // final destroy (or disable)
        Destroy(gameObject);
        yield break;
    }

    // Optional public API for taking damage directly (e.g., helper thrown objects can call this)
    public void TakeDirectDamage(float amount)
    {
        ApplyDamage(amount);
    }
}
