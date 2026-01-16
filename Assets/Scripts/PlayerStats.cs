/*
 * Script: PlayerStats.cs
 * Description: Manages player health (HP) and magic points (MP), including regeneration logic.
 * Updates UI and handles player death state.
 * * References:
 * - Unity Manual (Time.deltaTime): https://docs.unity3d.com/ScriptReference/Time-deltaTime.html
 * - Unity Manual (Mathf.Clamp): https://docs.unity3d.com/ScriptReference/Mathf.Clamp.html
 */

using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Magic")]
    public int maxMP = 100;
    public int currentMP;
    public float mpRegenRate = 4.0f;
    private float mpRegenAccumulator = 0.0f;

    void Start()
    {
        currentHP = maxHP;
        currentMP = maxMP;

        // Initialize UI at start
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdatePlayerHP(currentHP, maxHP);
            UIManager.instance.UpdatePlayerMP(currentMP, maxMP);
        }
    }

    void Update()
    {
        // Auto-Regenerate MP over time
        if (currentMP < maxMP)
        {
            mpRegenAccumulator += mpRegenRate * Time.deltaTime;

            // Add MP only when accumulator reaches 1 to keep integer values clean
            if (mpRegenAccumulator >= 1.0f)
            {
                int regenAmount = (int)mpRegenAccumulator;
                currentMP += regenAmount;
                mpRegenAccumulator -= regenAmount;

                // Clamp MP to max
                if (currentMP > maxMP) currentMP = maxMP;

                // Update UI after regeneration
                if (UIManager.instance != null)
                {
                    UIManager.instance.UpdatePlayerMP(currentMP, maxMP);
                }
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP < 0) currentHP = 0;

        // Update UI
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdatePlayerHP(currentHP, maxHP);
        }

        // Check for Death
        if (currentHP <= 0)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }

            // Detach camera so it doesn't disappear when the player object is disabled
            Camera mainCam = GetComponentInChildren<Camera>();
            if (mainCam != null)
            {
                mainCam.transform.SetParent(null);
            }

            // Disable player object
            gameObject.SetActive(false);
        }
    }

    public bool UseMP(int amountToUse)
    {
        if (currentMP >= amountToUse)
        {
            currentMP -= amountToUse;

            // Update UI
            if (UIManager.instance != null)
            {
                UIManager.instance.UpdatePlayerMP(currentMP, maxMP);
            }
            return true;
        }
        else
        {
            // Not enough MP
            return false;
        }
    }
}