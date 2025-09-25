using UnityEngine;

/// <summary>
/// Handles thrown object collisions. If it hits a zombie -> kill; if it hits Loom -> damage; otherwise behave normally.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ThrowableObject : MonoBehaviour
{
    public float damageToLoom = 8f;
    public float lifetimeAfterThrow = 6f;
    private Rigidbody2D rb;
    private bool thrown = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
    }

    private void Start()
    {
        // if objects are placed in world, they can be stationary (sleep)
    }

    public void ThrowTowards(Vector2 target, float force)
    {
        if (rb == null) return;
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        thrown = true;
        // Destroy after time to clean up
        Destroy(gameObject, lifetimeAfterThrow);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!thrown) return;

        // If hit a zombie
        ZombieWorkerAI zombie = collision.collider.GetComponent<ZombieWorkerAI>();
        if (zombie != null)
        {
            // instantly kill zombie
            zombie.Die(); // ensure ZombieWorkerAI has Die()
            Destroy(gameObject);
            return;
        }

        // If hit the Loom (trigger)
        LoomController loom = collision.collider.GetComponent<LoomController>();
        if (loom != null)
        {
            loom.TakeDirectDamage(damageToLoom);
            Destroy(gameObject);
            return;
        }

        // Otherwise, bounce or stick - simple behaviour: stop on first strong hit
        // Optionally reduce velocity
    }
}
