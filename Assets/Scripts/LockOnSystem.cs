/*
 * Script: LockOnSystem.cs
 * Description: Finds and locks onto the nearest enemy within the camera's view.
 * Includes Audio feedback when locking on.
 * * References:
 * - Unity Manual (Physics.OverlapSphere): https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html
 * - Unity Manual (Vector3.Dot): https://docs.unity3d.com/ScriptReference/Vector3.Dot.html
 * - Unity Manual (Transform.LookAt): https://docs.unity3d.com/ScriptReference/Transform.LookAt.html
 */

using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Required for sorting (OrderBy)

public class LockOnSystem : MonoBehaviour
{
    [Header("Settings")]
    // Reference to the player's transform to calculate distance.
    public Transform playerTransform;
    // Maximum range to detect enemies.
    public float maxLockOnDistance = 30.0f;
    // Layer mask to filter only "Enemy" objects.
    public LayerMask enemyLayer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip lockOnSound;

    // Internal variables to track state.
    private Transform currentTarget = null;
    private bool isLockedOn = false;

    // Public properties to allow other scripts to access the status.
    public bool IsLockedOn => isLockedOn;
    public Transform CurrentTarget => currentTarget;

    void Update()
    {
        // 1. Input Handling: Toggle lock-on with the Tab key.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isLockedOn)
            {
                ToggleLockOn(false); // Unlock
            }
            else
            {
                TryLockOn(); // Attempt to find a target
            }
        }

        // 2. Active Lock-on Logic: Update camera rotation.
        if (isLockedOn)
        {
            // Check if the target is null (destroyed) or out of range.
            if (currentTarget == null || Vector3.Distance(playerTransform.position, currentTarget.position) > maxLockOnDistance)
            {
                ToggleLockOn(false);
                return;
            }

            // Calculate the direction to the target.
            Vector3 targetDirection = currentTarget.position - transform.position;

            // Create a rotation looking at the target.
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Smoothly rotate the camera towards the target using Slerp.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
        }
    }

    // Searches for the nearest enemy.
    void TryLockOn()
    {
        // Find all colliders within the detection radius on the Enemy layer.
        Collider[] potentialTargets = Physics.OverlapSphere(playerTransform.position, maxLockOnDistance, enemyLayer);

        if (potentialTargets.Length > 0)
        {
            // Sort targets by distance to find the closest one using LINQ.
            var sortedTargets = potentialTargets
                .OrderBy(t => Vector3.Distance(playerTransform.position, t.transform.position))
                .ToArray();

            // Set the closest enemy as the current target.
            currentTarget = sortedTargets[0].transform;
            ToggleLockOn(true);
        }
    }

    // Toggles the lock-on state and handles audio/reset logic.
    void ToggleLockOn(bool state)
    {
        isLockedOn = state;

        if (state == true)
        {
            // Play sound effect on successful lock-on.
            if (audioSource != null && lockOnSound != null)
            {
                audioSource.PlayOneShot(lockOnSound);
            }
        }
        else
        {
            // Reset target and camera angle when unlocking.
            currentTarget = null;
            transform.localRotation = Quaternion.Euler(10f, 0, 0);
        }
    }
}