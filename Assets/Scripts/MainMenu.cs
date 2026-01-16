/*
 * Script: MainMenu.cs
 * Description: Handles main menu interactions (Start Game, Quit Game, Open/Close Panels).
 * Includes UI Click Sound.
 * * References:
 * - Unity Manual (SceneManager.LoadScene): https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadScene.html
 * - Unity Manual (Application.Quit): https://docs.unity3d.com/ScriptReference/Application.Quit.html
 */

using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    // Reference to the Controls/Instructions panel to toggle its visibility.
    public GameObject controlsPanel;

    [Header("Audio")]
    // Components for playing UI sound effects.
    public AudioSource audioSource;
    public AudioClip clickSound;

    // Helper method to play the click sound effect.
    // Checks if the audio source and clip are assigned before playing.
    public void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Called when the 'Start Game' button is pressed.
    // Plays a sound and loads the main game scene.
    public void PlayGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("rpgpp_lt_scene_1.0");
    }

    // Called when the 'Exit' button is pressed.
    // Plays a sound and quits the application.
    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Game Quit!"); // Log message for testing in the editor.
        Application.Quit();
    }

    // Shows the instructions panel when the 'Controls' button is clicked.
    public void ShowControls()
    {
        PlayClickSound();
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
    }

    // Hides the instructions panel when the 'Back' button is clicked.
    public void HideControls()
    {
        PlayClickSound();
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }
}