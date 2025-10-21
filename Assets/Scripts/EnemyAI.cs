using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyTankAI : MonoBehaviour
{
    #region Inspector Variables

    [Header("Movement Settings")]
    [Tooltip("The speed at which the enemy moves.")]
    [SerializeField] private float moveSpeed = 3.5f;
    [Tooltip("The radius around the tank for finding random roaming points when idle.")]
    [SerializeField] private float roamRadius = 15f;
    [Tooltip("The angular speed at which the tank rotates to face its target.")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 12f; 

    [Header("Attack Settings")]
    [Tooltip("The distance within which the enemy will detect and attack the player.")]
    [SerializeField] private float attackRange = 10f;
    [Tooltip("The time in seconds between each shot.")]
    [SerializeField] private float attackCooldown = 1.5f;
    [Tooltip("The amount of damage each bullet inflicts.")]
    [SerializeField] private int bulletDamage = 20;

    [Header("Targeting")]
    [Tooltip("The player target for this enemy tank. If null, the tank will roam randomly.")]
    [SerializeField] private Transform target;

    [Header("Required Components")]
    [Tooltip("The prefab for the bullet this tank will fire (must have a 'Bullet' script).")]
    [SerializeField] private GameObject bulletPrefab;
    [Tooltip("The transform from which bullets are fired.")]
    [SerializeField] private Transform firePoint;

    #endregion

    #region Private Variables

    private NavMeshAgent navMeshAgent;
    private float cooldownTimer;
    private float fixedY;

    #endregion

    #region Unity Lifecycle Methods

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
        }

        // Configure NavMeshAgent settings
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.stoppingDistance = attackRange * 0.9f;
        navMeshAgent.updateUpAxis = false; // Prevent vertical movement

        // Store initial Y position
        fixedY = transform.position.y;
    }

    private void Start()
    {
        if (firePoint == null)
        {
            Debug.LogError("Fire Point is not assigned on " + gameObject.name);
            enabled = false;
            return;
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefab is not assigned on " + gameObject.name);
            enabled = false;
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("No target assigned to " + gameObject.name + ". AI will roam randomly.");
        }

        cooldownTimer = attackCooldown;
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (target == null)
        {
            HandleRoamState();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange)
        {
            HandleAttackState();
        }
        else if (distanceToTarget <= detectionRange)
        {
            HandleChaseState();
        }
        else
        {
            HandleRoamState();
        }
    }

    private void LateUpdate()
    {
        // Keep tank grounded
        Vector3 pos = transform.position;
        pos.y = fixedY;
        transform.position = pos;
    }

    #endregion

    #region AI Logic

    private void HandleAttackState()
    {
        navMeshAgent.isStopped = true;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        directionToTarget.y = 0; 
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Dot(transform.forward, directionToTarget) > 0.95f)
        {
            if (cooldownTimer <= 0f)
            {
                Shoot();
                cooldownTimer = attackCooldown;
            }
        }
    }

    private void HandleChaseState()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(target.position);
    }

    private void HandleRoamState()
    {
        navMeshAgent.isStopped = false;

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh(transform.position, roamRadius);
            navMeshAgent.SetDestination(randomPoint);
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Bullet bulletScript = bulletObject.GetComponent<Bullet>(); 
        if (bulletScript != null)
        {
            Collider myCollider = GetComponent<Collider>();
            bulletScript.SetOwnerTag("Enemy", bulletDamage, myCollider);
        }

        Rigidbody rb = bulletObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * 25f;
        }
    }

    private Vector3 GetRandomPointOnNavMesh(Vector3 origin, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, radius, -1);
        return navHit.position;
    }

    #endregion

    #region Editor Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }

    #endregion
}
