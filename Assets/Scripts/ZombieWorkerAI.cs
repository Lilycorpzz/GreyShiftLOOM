using UnityEngine;
using System;
using System.Collections;

public class ZombieWorkerAI : MonoBehaviour
{
    public Action OnDestroyed;

    [Header("Movement")]
    public float wanderRadius = 3f;       // how far from spawn the zombie wanders
    public float patrolSpeed = 1.2f;
    public float chaseSpeed = 2.2f;
    public float reachThreshold = 0.1f;

    [Header("Detection/Combat")]
    public float detectionRadius = 6f;
    public float attackRange = 0.9f;
    public float attackInterval = 1.0f;
    public float sparkDamagePerHit = 5f;

    [Header("Misc")]
    public float idleWhenPlayerAITask = 1f; // not used directly, zombies just remain idle while player AITask = true

    private Rigidbody2D rb;
    private Transform player;
    private PlayerActionController playerAction;
    private Vector2 spawnCenter;
    private Vector2 currentTarget;
    private enum State { Patrol, Chase, Attack, IdleStand }
    private State state = State.Patrol;
    private Coroutine attackRoutine;
    private bool initialized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // kinematic movement using MovePosition
    }

    public void Initialize(Transform spawnPoint)
    {
        spawnCenter = spawnPoint.position;
        // pick first wander target
        PickNewWanderTarget();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerAction = player.GetComponent<PlayerActionController>();
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        // If player is doing AI tasks -> zombies do not look (idle)
        if (playerAction != null && playerAction.IsDoingAITask)
        {
            if (state != State.IdleStand)
            {
                state = State.IdleStand;
                StopAttack();
            }
        }
        else
        {
            // Normal behaviour: active patrol/search/attack
            float distToPlayer = (player != null) ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

            if (distToPlayer <= attackRange)
            {
                state = State.Attack;
            }
            else if (distToPlayer <= detectionRadius)
            {
                state = State.Chase;
                StopAttack();
            }
            else
            {
                // player not nearby: patrol
                state = State.Patrol;
                StopAttack();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!initialized) return;
        switch (state)
        {
            case State.IdleStand:
                // do nothing (stand still)
                break;

            case State.Patrol:
                DoPatrol();
                break;

            case State.Chase:
                DoChase();
                break;

            case State.Attack:
                DoAttack();
                break;
        }
    }

    private void DoPatrol()
    {
        Vector2 pos = rb.position;
        Vector2 dir = (currentTarget - pos);
        if (dir.magnitude <= reachThreshold)
        {
            // pick new wander target
            PickNewWanderTarget();
            dir = (currentTarget - pos);
        }
        Vector2 step = dir.normalized * patrolSpeed * Time.fixedDeltaTime;
        rb.MovePosition(pos + step);
    }

    private void DoChase()
    {
        if (player == null) return;
        Vector2 pos = rb.position;
        Vector2 dir = ((Vector2)player.position - pos);
        Vector2 move = dir.normalized * chaseSpeed * Time.fixedDeltaTime;
        rb.MovePosition(pos + move);
    }

    private void DoAttack()
    {
        // start attack coroutine if not already
        if (attackRoutine == null)
        {
            attackRoutine = StartCoroutine(AttackTick());
        }
    }

    private IEnumerator AttackTick()
    {
        while (true)
        {
            if (player == null || SparkManager.Instance == null) yield break;

            // If player is doing AI tasks mid-attack, stop attack
            if (playerAction != null && playerAction.IsDoingAITask)
            {
                attackRoutine = null;
                yield break;
            }

            // Do damage to player's spark
            SparkManager.Instance.ChangeSparkImmediate(-sparkDamagePerHit);

            // If player's spark reached 0, optionally trigger something (SparkManager has OnSparkDepleted)
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void StopAttack()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    private void PickNewWanderTarget()
    {
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * wanderRadius;
        currentTarget = spawnCenter + randomOffset;
    }

    // Cleanup - notify spawner / manager if destroyed
    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }

    // Optional: visual debug in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
