using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class CubicleInteractable : MonoBehaviour
{
    [Header("Task Settings")]
    [Tooltip("Total spark to remove while performing this AI task.")]
    public float sparkDrainAmount = 10f;
    [Tooltip("How long the AI task takes (seconds). The drain is spread across this duration).")]
    public float taskDuration = 1.2f;
    [Tooltip("Minimum cooldown before this cubicle can be used again.")]
    public float cooldown = 0.6f;

    private bool isOnCooldown = false;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Called by InteractionController when a player clicks this cubicle.
    /// </summary>
    public void Interact()
    {
        if (isOnCooldown) return;
        StartCoroutine(DoAITask());
    }

    private IEnumerator DoAITask()
    {
        isOnCooldown = true;

        // optional visual feedback: tint the cubicle briefly
        if (sr != null) sr.color = Color.Lerp(sr.color, Color.gray, 0.6f);

        // start draining spark over time using the SparkManager coroutine
        if (SparkManager.Instance != null)
        {
            yield return StartCoroutine(SparkManager.Instance.DrainSparkOverTime(sparkDrainAmount, taskDuration));
        }
        else
        {
            Debug.LogWarning("No SparkManager found in scene. Attach SparkManager to a GameObject.");
        }

        // restore visual
        if (sr != null) sr.color = Color.white;

        // enforce cooldown
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    // Optional: editor button to test
    [ContextMenu("Test Interact")]
    private void TestInteract() { Interact(); }
}
