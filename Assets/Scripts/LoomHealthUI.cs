using UnityEngine;
using UnityEngine.UI;

public class LoomHealthUI : MonoBehaviour
{
    public Slider loomSlider;
    public LoomController loom;

    private void Start()
    {
        if (loom == null) loom = FindObjectOfType<LoomController>();
        if (loom == null || loomSlider == null)
        {
            Debug.LogError("[LoomHealthUI] Missing references.");
            return;
        }

        loomSlider.minValue = 0f;
        loomSlider.maxValue = 1f;

        loom.OnHealthChanged += OnLoomHealthChanged;
        OnLoomHealthChanged(loom.GetNormalizedHealth());
    }

    private void OnDestroy()
    {
        if (loom != null)
            loom.OnHealthChanged -= OnLoomHealthChanged;
    }

    private void OnLoomHealthChanged(float normalized)
    {
        loomSlider.value = normalized;
    }
}
