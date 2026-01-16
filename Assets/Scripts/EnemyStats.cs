/*
 * Script: EnemyStats.cs
 * Description: Manages enemy health (HP) and handles death logic.
 * Updates the UI and notifies GameManager upon death.
 * * References:
 * - Unity Manual (MonoBehaviour): https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
 * - Unity Manual (Object.Destroy): https://docs.unity3d.com/ScriptReference/Object.Destroy.html
 */

using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 100;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;

        // Initialize Enemy HP bar at start
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateEnemyHP(currentHP, maxHP);
        }
    }

    // Called when the enemy is hit by a fireball
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        // Update Enemy HP bar UI
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateEnemyHP(currentHP, maxHP);
        }

        Debug.Log("Enemy took damage! HP: " + currentHP);

        // Check for death
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Notify GameManager that the player has won
        if (GameManager.instance != null)
        {
            GameManager.instance.Victory();
        }

        // Remove enemy object from the scene
        Destroy(gameObject);
    }
}