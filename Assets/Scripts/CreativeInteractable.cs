using UnityEngine;

public class CreativeInteractable : MonoBehaviour
{
    [Header("Creative Task Settings")]
    public float sparkRestoreAmount = 20f;  // total spark to restore
    public float taskDuration = 2.0f;       // seconds
    public float cooldown = 1.0f;

    private bool isOnCooldown = false;
    private SpriteRenderer sr;
    private PlayerActionController playerAction;

    private void Awake() => sr = GetComponent<SpriteRenderer>();

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerAction = player.GetComponent<PlayerActionController>();
        if (playerAction == null)
            Debug.LogWarning("[CreativeInteractable] PlayerActionController not found on 'Player' tag.");
    }

    public void Interact()
    {
        if (isOnCooldown) return;
        StartCoroutine(DoCreativeTask());
    }

    private IEnumerator DoCreativeTask()
    {
        isOnCooldown = true;
        if (playerAction != null) playerAction.SetDoingCreativeTask(true);

        // visual: tint to warmer color while doing creative work
        if (sr != null) sr.color = Color.Lerp(sr.color, new Color(1f, 0.85f, 0.6f), 0.6f);

        if (SparkManager.Instance != null)
        {
            yield return StartCoroutine(SparkManager.Instance.RestoreSparkOverTime(sparkRestoreAmount, taskDuration));
        }

        // restore visual
        if (sr != null) sr.color = Color.white;

        if (playerAction != null) playerAction.SetDoingCreativeTask(false);

        // Small cooldown after creative action
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    [ContextMenu("Test Interact")]
    private void TestInteract() { Interact(); }
}
