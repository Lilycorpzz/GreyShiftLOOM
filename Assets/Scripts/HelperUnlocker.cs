using UnityEngine;

public class HelperUnlocker : MonoBehaviour
{
    [Range(0f, 1f)]
    public float unlockThreshold = 0.8f; // when normalized spark >= this value, unlock helper
    private bool unlocked = false;
    public NotificationManager notificationManager;

    private void Start()
    {
        if (SparkManager.Instance != null)
            SparkManager.Instance.OnSparkChanged += OnSparkChanged;
    }

    private void OnDestroy()
    {
        if (SparkManager.Instance != null)
            SparkManager.Instance.OnSparkChanged -= OnSparkChanged;
    }

    private void OnSparkChanged(float normalized)
    {
        if (!unlocked && normalized >= unlockThreshold)
        {
            unlocked = true;
            if (notificationManager != null)
                notificationManager.ShowNotification("Helper available — press [H] to add helper (later).");
        }
    }
}
