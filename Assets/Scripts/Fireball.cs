/*
 * Script: Fireball.cs
 * Description: Controls the projectile movement and collision logic using Triggers.
 * Handles damage application, self-damage prevention, and explosion effects/sounds.
 * * References:
 * - Unity Manual (OnTriggerEnter): https://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html
 * - Unity Manual (Rigidbody.linearVelocity): https://docs.unity3d.com/ScriptReference/Rigidbody-linearVelocity.html
 * - Unity Manual (AudioSource.PlayClipAtPoint): https://docs.unity3d.com/ScriptReference/AudioSource.PlayClipAtPoint.html
 */

using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 15.0f;
    public float lifeTime = 3.0f;
    public int damage = 20;
    public GameObject owner; // The character who shot this fireball (to prevent self-damage)

    [Header("Effects")]
    public GameObject explosionPrefab;

    [Header("Audio")] // Added for SFX
    public AudioClip explosionSound;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        // Apply forward velocity
        // Note: 'linearVelocity' is the new API in Unity 6 (formerly 'velocity')
        rb.linearVelocity = transform.forward * speed;

        // Auto-destroy bullet after lifetime to save performance
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Hit Shield
        if (other.CompareTag("Shield"))
        {
            Explode();
            Destroy(gameObject);
            return;
        }

        // 2. Hit Enemy
        EnemyStats enemy = other.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            // Self-damage check: Do not damage the shooter
            if (owner != null && owner == other.gameObject) return;

            enemy.TakeDamage(damage);
            Explode();
            Destroy(gameObject);
            return;
        }

        // 3. Hit Player
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            // Self-damage check
            if (owner != null && owner == other.gameObject) return;

            player.TakeDamage(10);
            Explode();
            Destroy(gameObject);
            return;
        }

        // 4. Hit Environment (Walls, Floor, etc.)
        // Destroy fireball if it hits something that isn't the shooter
        if (!other.CompareTag("Player") && !other.CompareTag("Enemy"))
        {
            Explode();
            Destroy(gameObject);
        }
    }

    // Helper function to handle visual and audio effects on impact
    void Explode()
    {
        // Play sound at the position of impact
        // We use PlayClipAtPoint because the fireball object is about to be destroyed,
        // so a normal AudioSource would stop playing immediately.
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1.0f);
        }

        // Instantiate visual explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }
    }
}