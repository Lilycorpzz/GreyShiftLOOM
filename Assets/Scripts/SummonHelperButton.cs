using UnityEngine;
using UnityEngine.UI;

public class SummonHelperButton : MonoBehaviour
{
    public Button button;
    public GameObject helperPrefab;     // assign Helper_Prefab
    public Transform helperSpawnPoint;  // e.g., player transform or a spawn empty
    [Range(0f, 1f)] public float unlockThreshold = 0.8f; // normalized spark required

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.interactable = false;
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnEnable()
    {
        if (SparkManager.Instance != null)
            SparkManager.Instance.OnSparkChanged += OnSparkChanged;
    }

    private void OnDisable()
    {
        if (SparkManager.Instance != null)
            SparkManager.Instance.OnSparkChanged -= OnSparkChanged;
    }

    private void OnSparkChanged(float normalized)
    {
        // enable when spark >= threshold
        button.interactable = normalized >= unlockThreshold;
    }

    private void OnButtonClicked()
    {
        if (helperPrefab == null)
        {
            Debug.LogWarning("Helper prefab not assigned to SummonHelperButton.");
            return;
        }

        Transform spawnAt = helperSpawnPoint != null ? helperSpawnPoint : Camera.main.transform;
        Vector3 pos = spawnAt.position;
        // Instantiate helper at spawn position
        Instantiate(helperPrefab, pos, Quaternion.identity);
        // optionally disable button to prevent spamming
        button.interactable = false;
    }
}
