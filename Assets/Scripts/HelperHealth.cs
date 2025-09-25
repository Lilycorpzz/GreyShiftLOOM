using UnityEngine;

public class HelperHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float current;

    private void Awake()
    {
        current = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        current -= amount;
        if (current <= 0f)
            Die();
    }

    private void Die()
    {
        // find HelperController and call OnDeath
        HelperController hc = GetComponent<HelperController>();
        if (hc != null) hc.OnDeath();
        else Destroy(gameObject);
    }
}
