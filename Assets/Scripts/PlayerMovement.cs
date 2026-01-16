/*
 * Script: PlayerMovement.cs
 * Description: Handles player movement, rotation, and GRAVITY.
 * Includes logic for switching between Normal movement and Locked-on Strafing movement.
 */

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.0f;

    [Header("Gravity")]
    public float gravity = -20.0f; // Gravity strength

    [Header("Rotation")]
    public float rotateSpeed = 100.0f;

    [Header("References")]
    public Transform mainCameraTransform;

    private CharacterController controller;
    private LockOnSystem lockOnSystem; // Reference to LockOnSystem

    private Vector3 velocity; // Stores vertical velocity for gravity
    private bool isGrounded;  // Check if player is on ground

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Get LockOnSystem from Main Camera
        if (mainCameraTransform != null)
        {
            lockOnSystem = mainCameraTransform.GetComponent<LockOnSystem>();
        }
    }

    void Update()
    {
        // --- 0. Ground Check ---
        // Resets vertical velocity when on the ground to prevent infinite falling speed accumulation
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep player snapped to ground
        }

        // --- 1. Get Input ---
        float moveVertical = Input.GetAxis("Vertical");   // W/S
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D

        Vector3 moveDirection = Vector3.zero;

        // --- 2. Check Lock-on State ---
        if (lockOnSystem != null && lockOnSystem.IsLockedOn && lockOnSystem.CurrentTarget != null)
        {
            // --- Locked-on Movement (Strafing) ---

            // (A) Rotate player to face the target automatically
            HandleLockOnRotation();

            // (B) Move relative to Camera (Strafing)
            Vector3 camForward = mainCameraTransform.forward;
            Vector3 camRight = mainCameraTransform.right;

            camForward.y = 0; // Ignore vertical tilt
            camRight.y = 0;

            moveDirection = (camForward.normalized * moveVertical) + (camRight.normalized * moveHorizontal);
        }
        else
        {
            // --- Normal Movement (Tank Controls) ---

            // (A) Manual Rotation using Q/E keys
            float rotateInput = 0f;
            if (Input.GetKey(KeyCode.E)) rotateInput = 1f;
            else if (Input.GetKey(KeyCode.Q)) rotateInput = -1f;

            transform.Rotate(0f, rotateInput * rotateSpeed * Time.deltaTime, 0f);

            // (B) Move relative to Player's own forward direction
            moveDirection = (transform.forward * moveVertical) + (transform.right * moveHorizontal);
        }

        // --- 3. Apply Movement (Horizontal) ---
        // Normalize direction to prevent faster diagonal movement
        if (moveDirection.magnitude > 1f) moveDirection.Normalize();

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // --- 4. Apply Gravity (Vertical) ---
        // Calculate gravity (v = v0 + at)
        velocity.y += gravity * Time.deltaTime;

        // Apply vertical movement
        controller.Move(velocity * Time.deltaTime);
    }

    // Helper to face the target while locked on
    void HandleLockOnRotation()
    {
        // Calculate direction to target
        Vector3 directionToTarget = lockOnSystem.CurrentTarget.position - transform.position;
        directionToTarget.y = 0; // Keep rotation flat (Y-axis only)

        // Smoothly rotate towards target
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
        }
    }
}