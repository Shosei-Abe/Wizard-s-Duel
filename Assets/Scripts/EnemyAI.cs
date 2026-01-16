/*
 * Script: EnemyAI.cs
 * Description: Controls the enemy behavior using NavMeshAgent (Chase, Attack, Evade, Defend).
 * Includes "Human-like" imperfections (Reaction time & Mistake chance).
 */

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Combat (Attack)")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float attackRange = 15.0f;
    public float attackCooldown = 2.0f;

    [Header("Combat (Defense)")]
    public GameObject shieldPrefab;
    public float detectionRadius = 4.0f;

    // --- Added: Human-like behavior settings ---
    [Header("AI Difficulty (Humanize)")]
    [Tooltip("Delay in seconds before blocking after detecting a projectile")]
    public float reactionTime = 0.3f;
    [Tooltip("Probability of successfully blocking (0.0 to 1.0)")]
    public float blockChance = 0.75f; // 75% chance to block

    private float currentReactionTimer = 0f; // Internal timer for reaction logic
    // -------------------------------------------

    // Defense Cooldown Settings
    public float shieldCooldown = 0.2f;
    private float shieldCooldownTimer = 0f;
    private GameObject currentShield = null;

    [Header("Combat (Evasion)")]
    public float safeDistance = 8.0f;
    public float teleportDistance = 25.0f;
    public float teleportCooldown = 3.0f;
    public GameObject teleportEffectPrefab;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip teleportSound;

    private NavMeshAgent agent;
    private float attackTimer = 0f;
    private float teleportTimer = 0f;
    private PlayerMagic playerMagic;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Initialize reaction timer at start
        currentReactionTimer = reactionTime;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerMagic = player.GetComponent<PlayerMagic>();
            }
        }
        if (firePoint == null) firePoint = transform;
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        // Update Timers
        if (teleportTimer > 0) teleportTimer -= Time.deltaTime;
        if (shieldCooldownTimer > 0) shieldCooldownTimer -= Time.deltaTime;

        // Check for defensive actions (Shield) with Human Delay
        HandleDefense();

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // --- 1. Evasion Logic (Teleport) ---
        if (distance < safeDistance && teleportTimer <= 0)
        {
            if (TryEvadeTeleport()) return;
        }

        // --- 2. Combat Logic (Attack & Chase) ---
        if (distance <= attackRange)
        {
            agent.isStopped = true;
            FaceTarget();
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                // Attack only if neither player nor enemy is shielding
                if ((playerMagic == null || !playerMagic.IsShieldActive) && currentShield == null)
                {
                    Attack();
                    attackTimer = 0f;
                }
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    bool TryEvadeTeleport()
    {
        Vector3 directionAway = (transform.position - player.position).normalized;

        Vector3[] tryDirections = new Vector3[]
        {
            directionAway,
            Quaternion.Euler(0, 45, 0) * directionAway,
            Quaternion.Euler(0, -45, 0) * directionAway,
            Quaternion.Euler(0, 90, 0) * directionAway,
            Quaternion.Euler(0, -90, 0) * directionAway,
            Quaternion.Euler(0, 135, 0) * directionAway,
            Quaternion.Euler(0, -135, 0) * directionAway
        };

        foreach (var dir in tryDirections)
        {
            Vector3 targetPos = transform.position + dir * teleportDistance;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPos, out hit, 20.0f, NavMesh.AllAreas))
            {
                if (Vector3.Distance(transform.position, hit.position) > 3.0f)
                {
                    DoTeleport(hit.position);
                    return true;
                }
            }
        }

        Vector3 randomPoint = transform.position + Random.insideUnitSphere * teleportDistance;
        NavMeshHit randomHit;
        if (NavMesh.SamplePosition(randomPoint, out randomHit, 20.0f, NavMesh.AllAreas))
        {
            if (Vector3.Distance(transform.position, randomHit.position) > 3.0f)
            {
                DoTeleport(randomHit.position);
                return true;
            }
        }
        return false;
    }

    void DoTeleport(Vector3 pos)
    {
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position + Vector3.up, transform.rotation);
            Instantiate(teleportEffectPrefab, pos + Vector3.up, transform.rotation);
        }

        if (audioSource != null && teleportSound != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        agent.Warp(pos);
        Debug.Log("Enemy Teleported Long Range!");
        teleportTimer = teleportCooldown;
        attackTimer = 0f;
    }

    // --- Modified: Human-like Defense Logic ---
    void HandleDefense()
    {
        // Ignore if on cooldown
        if (shieldCooldownTimer > 0) return;

        // Search for projectiles
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        bool threatDetected = false;

        foreach (var hit in hits)
        {
            Fireball incoming = hit.GetComponent<Fireball>();
            // Consider it a threat if it's not my own fireball
            if (incoming != null && incoming.owner != gameObject)
            {
                threatDetected = true;
                break;
            }
        }

        if (threatDetected)
        {
            // If threat detected, reduce reaction timer instead of blocking immediately
            currentReactionTimer -= Time.deltaTime;

            // Proceed to shield logic only when reaction timer hits zero
            if (currentReactionTimer <= 0)
            {
                // Check probability and block only if no shield is currently active
                if (currentShield == null && shieldPrefab != null)
                {
                    // Block only if chance succeeds (if failed, do not block = take damage)
                    if (Random.value <= blockChance)
                    {
                        currentShield = Instantiate(shieldPrefab, transform.position, transform.rotation);
                        currentShield.transform.SetParent(transform);
                    }
                    else
                    {
                        // If failed, do nothing (enemy will stand still and take damage)
                        Debug.Log("Enemy failed to block! (Human error)");
                    }
                }
            }
        }
        else
        {
            // If no threat, deactivate shield & reset reaction timer
            if (currentShield != null)
            {
                Destroy(currentShield);
                currentShield = null;
                shieldCooldownTimer = shieldCooldown;
            }

            // When threat is gone, reset reaction timer for the next projectile
            currentReactionTimer = reactionTime;
        }
    }

    void Attack()
    {
        if (fireballPrefab != null)
        {
            Vector3 targetPos = player.position + Vector3.up * 1.0f;
            Vector3 direction = targetPos - firePoint.position;
            Quaternion rotation = Quaternion.LookRotation(direction);

            GameObject fb = Instantiate(fireballPrefab, firePoint.position, rotation);
            Fireball fbScript = fb.GetComponent<Fireball>();
            if (fbScript != null)
            {
                fbScript.owner = gameObject;
            }

            if (audioSource != null && attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}