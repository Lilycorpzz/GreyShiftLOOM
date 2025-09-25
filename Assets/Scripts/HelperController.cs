using UnityEngine;
using System.Collections;
using System.Linq;

public class HelperController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.2f;
    public float pickUpRange = 1.0f;
    public float searchRadius = 10f;

    [Header("Throwing")]
    public float throwForce = 8f;
    public float throwCooldown = 1.2f;

    [Header("References")]
    public Transform holdPoint; // optional child transform where helper holds the item

    private Rigidbody2D rb;
    private GameObject heldObject;
    private bool canThrow = true;
    private Transform loomTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // ensure dynamic rigidbody
        rb.isKinematic = false;
        rb.gravityScale = 0f;
    }

    private void Start()
    {
        // find Loom transform in scene
        LoomController loom = FindObjectOfType<LoomController>();
        if (loom != null) loomTransform = loom.transform;

        // if holdPoint missing create a small child
        if (holdPoint == null)
        {
            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(transform);
            hp.transform.localPosition = Vector3.up * 0.3f;
            holdPoint = hp.transform;
        }
    }

    private void Update()
    {
        // if currently holding an object, go to loom and throw
        if (heldObject != null)
        {
            if (loomTransform == null)
            {
                // idle wander if no loom
                MoveWander();
                return;
            }

            float dist = Vector2.Distance(transform.position, loomTransform.position);
            // move closer if far
            if (dist > 3f)
            {
                MoveTowards(loomTransform.position);
            }
            else
            {
                // close enough to throw
                if (canThrow)
                    StartCoroutine(ThrowRoutine());
            }

            // keep heldObject at holdPoint
            heldObject.transform.position = holdPoint.position;
            return;
        }

        // not holding - search for closest throwable
        GameObject target = FindClosestThrowable();
        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist <= pickUpRange)
            {
                PickUp(target);
            }
            else
            {
                MoveTowards(target.transform.position);
            }
        }
        else
        {
            // nothing found - wander idly
            MoveWander();
        }
    }

    private void MoveTowards(Vector2 worldPos)
    {
        Vector2 dir = ((Vector2)worldPos - rb.position).normalized;
        Vector2 newPos = rb.position + dir * moveSpeed * Time.deltaTime;
        rb.MovePosition(newPos);
    }

    private void MoveWander()
    {
        // simple idle behaviour: small random jitter
        rb.linearVelocity = Vector2.zero;
    }

    private GameObject FindClosestThrowable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        float best = float.MaxValue;
        GameObject bestGO = null;
        foreach (var c in hits)
        {
            if (c.CompareTag("Throwable"))
            {
                float d = Vector2.Distance(transform.position, c.transform.position);
                if (d < best)
                {
                    best = d;
                    bestGO = c.gameObject;
                }
            }
        }
        return bestGO;
    }

    private void PickUp(GameObject go)
    {
        heldObject = go;
        // disable physics while carried
        Rigidbody2D orb = heldObject.GetComponent<Rigidbody2D>();
        if (orb != null) orb.linearVelocity = Vector2.zero;

        Collider2D col = heldObject.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        heldObject.transform.position = holdPoint.position;
    }

    private IEnumerator ThrowRoutine()
    {
        canThrow = false;
        // small wind-up
        yield return new WaitForSeconds(0.1f);

        if (heldObject != null && loomTransform != null)
        {
            // re-enable physics, detach and throw
            Collider2D col = heldObject.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
            Rigidbody2D r = heldObject.GetComponent<Rigidbody2D>();
            if (r != null) r.isKinematic = false;

            // call the throwable script to apply force
            ThrowableObject throwable = heldObject.GetComponent<ThrowableObject>();
            if (throwable != null)
            {
                throwable.ThrowTowards(loomTransform.position, throwForce);
            }
            else
            {
                // fallback: apply direct force
                if (r != null)
                {
                    Vector2 dir = (loomTransform.position - heldObject.transform.position).normalized;
                    r.AddForce(dir * throwForce, ForceMode2D.Impulse);
                }
            }

            heldObject = null;
        }

        // cooldown between throws
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }

    // Helper can be damaged by zombies. Provide public function to apply damage.
    public void ApplyDamage(float amount)
    {
        HelperHealth hh = GetComponent<HelperHealth>();
        if (hh != null) hh.TakeDamage(amount);
    }

    // called when helper dies
    public void OnDeath()
    {
        // optional: spawn VFX, disable AI
        Destroy(gameObject);
    }

    // Gizmos for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);
    }
}
