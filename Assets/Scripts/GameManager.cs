/*
 * Script: GameManager.cs
 * Description: Manages the game state (Win/Loss conditions, Pause) and scene transitions.
 * Implements the Singleton pattern for global access.
 * * References:
 * - Unity Manual (SceneManager): https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html
 * - Unity Manual (Time.timeScale): https://docs.unity3d.com/ScriptReference/Time-timeScale.html
 * - Singleton Pattern in Unity: https://learn.unity.com/tutorial/implement-data-persistence-between-scenes
 */

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton Pattern: Accessible from anywhere
    public static GameManager instance;

    public bool isGameOver = false;
    private bool isPaused = false;

    void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Debug: Quick retry with 'R' key only when game is over
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RetryGame();
        }

        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            TogglePause();
        }
    }

    // Handles the logic for pausing/unpausing the game
    public void TogglePause()
    {
        isPaused = !isPaused;

        // Stop or Resume time (0 = stop, 1 = normal speed)
        Time.timeScale = isPaused ? 0f : 1f;

        // Show/Hide Pause UI via UIManager
        if (UIManager.instance != null)
        {
            UIManager.instance.TogglePausePanel(isPaused);
        }

        // Unlock cursor when paused so the player can click buttons
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    // Called when the player loses all HP
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("GAME OVER... You Lost.");

        // Show Defeat UI
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowResult("DEFEAT");
        }

        // Unlock cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Called when the enemy is defeated
    public void Victory()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("VICTORY! You Won!");

        // Show Victory UI
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowResult("VICTORY");
        }

        // Unlock cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // UI Function: Resume from Pause Menu
    public void ResumeGame()
    {
        // Force unpause
        if (isPaused)
        {
            TogglePause();
        }
    }

    // UI Function: Return to Main Menu
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Ensure time is running before changing scenes
        SceneManager.LoadScene("MainMenu");
    }

    // UI Function: Restart the current level
    public void RetryGame()
    {
        Time.timeScale = 1f; // Ensure time is running
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}