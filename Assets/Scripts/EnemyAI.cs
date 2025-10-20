using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the AI behavior for an enemy tank in a top-down MOBA-style game.
/// Handles movement, target acquisition, and attacking logic using a NavMeshAgent.
/// This version targets a single, pre-assigned Transform.
/// </summary>
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
    [SerializeField] private float detectionRange = 12f; // How far the enemy can "see" the player

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

    #endregion

    #region Unity Lifecycle Methods

    private void Awake()
    {
        // Get references to required components
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
        }

        // Configure NavMeshAgent settings
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.stoppingDistance = attackRange * 0.9f; // Stop just within attack range for reliability
    }

    private void Start()
    {
        // Ensure the fire point is assigned
        if (firePoint == null)
        {
            Debug.LogError("Fire Point is not assigned on " + gameObject.name);
            enabled = false; // Disable script if not configured correctly
            return;
        }

        // Ensure the bullet prefab is assigned
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

        // Set initial state
        cooldownTimer = attackCooldown;
    }

    private void Update()
{
    cooldownTimer -= Time.deltaTime;

    // If no player is assigned, just roam
    if (target == null)
    {
        HandleRoamState();
        return;
    }

    // Calculate distance to player
    float distanceToTarget = Vector3.Distance(transform.position, target.position);

    if (distanceToTarget <= attackRange)
    {
        HandleAttackState();  // Stop and shoot
    }
    else if (distanceToTarget <= detectionRange)
    {
        HandleChaseState();   // Move toward player but not shooting yet
    }
    else
    {
        HandleRoamState();    // Player is too far, roam randomly
    }
}


    #endregion

    #region AI Logic

    /// <summary>
    /// Manages behavior when a target is in range: stop, face the target, and shoot.
    /// </summary>
    private void HandleAttackState()
    {
        navMeshAgent.isStopped = true;

        // --- Rotation ---
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        directionToTarget.y = 0; // Keep rotation on the horizontal plane
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // --- Shooting ---
        // Check if the tank is roughly facing the target before shooting
        if (Vector3.Dot(transform.forward, directionToTarget) > 0.95f)
        {
            if (cooldownTimer <= 0f)
            {
                Shoot();
                cooldownTimer = attackCooldown;
            }
        }
    }
    
    /// <summary>
    /// Moves the AI towards the current target's position.
    /// </summary>
    private void HandleChaseState()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(target.position);
    }

    /// <summary>
    /// Manages behavior when no target is present or assigned: roam randomly.
    /// </summary>
    private void HandleRoamState()
    {
        navMeshAgent.isStopped = false;

        // If the agent is idle (not calculating a path and has reached its destination), find a new roam point.
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh(transform.position, roamRadius);
            navMeshAgent.SetDestination(randomPoint);
        }
    }
    
    /// <summary>
    /// Instantiates a bullet and sets its owner tag and damage.
    /// </summary>
   private void Shoot()
{
    if (bulletPrefab == null || firePoint == null) return;

    // Instantiate bullet
    GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    // Get the Bullet component
    Bullet bulletScript = bulletObject.GetComponent<Bullet>(); 
    if (bulletScript != null)
    {
        // Pass explicit owner tag and tank collider
        Collider myCollider = GetComponent<Collider>();
        bulletScript.SetOwnerTag("Enemy", bulletDamage, myCollider);
    }
    else
    {
        Debug.LogWarning("Bullet prefab is missing a 'Bullet' script with a 'SetOwnerTag' method.");
    }

    // Optionally, add velocity if bullet uses Rigidbody
    Rigidbody rb = bulletObject.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.linearVelocity = firePoint.forward * 25f; // adjust speed as needed
    }
}

    /// <summary>
    /// Finds a random reachable point on the NavMesh within a given radius.
    /// </summary>
    /// <param name="origin">The center of the search area.</param>
    /// <param name="radius">The radius of the search area.</param>
    /// <returns>A random point on the NavMesh.</returns>
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

    // Visualize the attack and roam ranges in the editor for easier setup.
    private void OnDrawGizmosSelected()
    {
        // Draw Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw Roam Radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }

    #endregion
}

