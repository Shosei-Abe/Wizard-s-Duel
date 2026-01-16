/*
 * Script: PlayerMagic.cs
 * Description: Handles player's magic abilities (Fireball, Shield, Teleport) and MP consumption.
 * Includes Lock-on targeting logic and Audio feedback.
 * * References:
 * - Unity Manual (Instantiate): https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
 * - Unity Manual (AudioSource): https://docs.unity3d.com/ScriptReference/AudioSource.html
 * - Unity Manual (ParticleSystem): https://docs.unity3d.com/Manual/PartSysMainModule.html
 * - Unity Manual (Quaternion.LookRotation): https://docs.unity3d.com/ScriptReference/Quaternion.LookRotation.html
 */

using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    private PlayerStats playerStats;
    private CharacterController controller;

    [Header("References")]
    public Transform mainCameraTransform;
    private LockOnSystem lockOnSystem; // Reference to LockOnSystem

    [Header("Fireball")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public int fireballCost = 6;

    [Header("Shield")]
    public GameObject shieldPrefab;
    public int shieldCostPerSecond = 5;
    private GameObject currentShield = null;
    private float shieldMPDrain = 0.0f;

    [Header("Teleport")]
    public float teleportDistance = 7.0f;
    public int teleportCost = 20;
    public GameObject teleportEffectPrefab;

    [Header("Audio")] // Added for SFX
    public AudioSource audioSource;
    public AudioClip fireballSound;
    public AudioClip shieldSound;
    public AudioClip teleportSound;

    public bool IsShieldActive => currentShield != null;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        controller = GetComponent<CharacterController>();

        // Get LockOnSystem from Main Camera
        if (mainCameraTransform != null)
        {
            lockOnSystem = mainCameraTransform.GetComponent<LockOnSystem>();
        }

        if (fireballSpawnPoint == null)
        {
            fireballSpawnPoint = transform;
        }
    }

    void Update()
    {
        // 1. Cast Fireball
        if (Input.GetKeyDown(KeyCode.F))
        {
            HandleFireball();
        }

        // 2. Toggle Shield (Hold R)
        HandleShield();

        // 3. Teleport (Space)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleTeleport();
        }
    }

    void HandleFireball()
    {
        if (playerStats.UseMP(fireballCost))
        {
            if (fireballPrefab != null)
            {
                Quaternion fireballRotation; // Launch angle

                // Check if locked onto a target
                if (lockOnSystem != null && lockOnSystem.IsLockedOn && lockOnSystem.CurrentTarget != null)
                {
                    // (A) If Locked On: Calculate direction to target center
                    // Aim at chest height (Y + 1.0f)
                    Vector3 targetCenter = lockOnSystem.CurrentTarget.position + Vector3.up * 1.0f;

                    // Calculate direction vector
                    Vector3 directionToTarget = targetCenter - fireballSpawnPoint.position;

                    // Rotate fireball towards target
                    fireballRotation = Quaternion.LookRotation(directionToTarget);
                }
                else
                {
                    // (B) If Not Locked On: Fire straight forward
                    fireballRotation = fireballSpawnPoint.rotation;
                }

                // Instantiate Fireball
                GameObject fb = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballRotation);
                Fireball fbScript = fb.GetComponent<Fireball>();
                if (fbScript != null)
                {
                    fbScript.owner = gameObject; // Set owner to prevent self-damage
                }

                // Play Sound
                PlaySound(fireballSound);

                Debug.Log("Fireball cast!");
            }
        }
    }

    void HandleShield()
    {
        if (Input.GetKey(KeyCode.R))
        {
            // Create Shield if not active
            if (currentShield == null)
            {
                if (playerStats.currentMP > 1 && shieldPrefab != null)
                {
                    Vector3 spawnPos = transform.position + controller.center;
                    currentShield = Instantiate(shieldPrefab, spawnPos, transform.rotation);
                    currentShield.transform.SetParent(transform);

                    // Play Sound
                    PlaySound(shieldSound);
                }
            }

            // Maintain Shield (Drain MP over time)
            if (currentShield != null)
            {
                shieldMPDrain += shieldCostPerSecond * Time.deltaTime;
                if (shieldMPDrain >= 1.0f)
                {
                    int cost = (int)shieldMPDrain;
                    if (playerStats.UseMP(cost))
                    {
                        shieldMPDrain -= cost;
                    }
                    else
                    {
                        // Not enough MP, destroy shield
                        Destroy(currentShield);
                        currentShield = null;
                        shieldMPDrain = 0.0f;
                    }
                }
            }
        }
        else
        {
            // Release Shield key
            if (currentShield != null)
            {
                Destroy(currentShield);
                currentShield = null;
                shieldMPDrain = 0.0f;
            }
        }
    }

    void HandleTeleport()
    {
        if (playerStats.UseMP(teleportCost))
        {
            Vector3 startPos = transform.position;
            Vector3 teleportVector = transform.forward * teleportDistance;
            Vector3 endPos = startPos + teleportVector; // Destination

            // Effect at start position
            if (teleportEffectPrefab != null)
            {
                Instantiate(teleportEffectPrefab, startPos + Vector3.up, transform.rotation);
            }

            // Play Sound
            PlaySound(teleportSound);

            // Move Player
            controller.enabled = false;
            transform.position += teleportVector;
            controller.enabled = true;

            // Effect at end position
            if (teleportEffectPrefab != null)
            {
                Instantiate(teleportEffectPrefab, endPos + Vector3.up, transform.rotation);
            }
            Debug.Log("Teleported!");
        }
    }

    // Helper function to play sound
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}