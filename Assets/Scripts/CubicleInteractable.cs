using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class CubicleInteractable : MonoBehaviour
{
    [Header("Task Settings")]
    public float sparkDrainAmount = 10f;
    public float taskDuration = 1.2f;
    public float cooldown = 0.6f;

    private bool isOnCooldown = false;
    private SpriteRenderer sr;
    private PlayerActionController playerAction;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Try to find the player by tag.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerAction = player.GetComponent<PlayerActionController>();
        if (playerAction == null)
            Debug.LogWarning("[CubicleInteractable] PlayerActionController not found on object tagged 'Player'.");
    }

    public void Interact()
    {
        if (isOnCooldown) return;
        StartCoroutine(DoAITaskRoutine());
    }

    private IEnumerator DoAITaskRoutine()
    {
        isOnCooldown = true;

        // Set player into AI task state so zombies go idle
        if (playerAction != null) playerAction.SetDoingAITask(true);

        // visual feedback: dim cubicle
        if (sr != null) sr.color = Color.Lerp(sr.color, Color.gray, 0.6f);

        // drain spark over the taskDuration
        if (SparkManager.Instance != null)
        {
            yield return StartCoroutine(SparkManager.Instance.DrainSparkOverTime(sparkDrainAmount, taskDuration));
        }

        // restore visual
        if (sr != null) sr.color = Color.white;

        // End AI task state
        if (playerAction != null) playerAction.SetDoingAITask(false);

        // cooldown before re-use
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    [ContextMenu("Test Interact")]
    private void TestInteract() { Interact(); }
}
