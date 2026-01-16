/*
 * Script: UIManager.cs
 * Description: Updates the UI elements (Sliders, Text) based on game events.
 * Manages the visibility of the Pause Panel and Result Screen.
 * * References:
 * - Unity Manual (UI Slider): https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-Slider.html
 * - TextMeshPro Documentation: https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html
 * - Unity Manual (GameObject.SetActive): https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html
 */

using UnityEngine;
using UnityEngine.UI; // Required for Slider
using TMPro; // Required for TextMeshPro

public class UIManager : MonoBehaviour
{
    // Singleton Instance
    public static UIManager instance;

    [Header("Pause UI")]
    public GameObject pausePanel;

    [Header("Player UI")]
    public Slider playerHPSlider;
    public Slider playerMPSlider;

    [Header("Enemy UI")]
    public Slider enemyHPSlider;

    [Header("Result UI")]
    public GameObject resultPanel; // Controls the whole panel, not just text
    public TextMeshProUGUI resultText;

    void Awake()
    {
        // Singleton Pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Pause Functions ---

    // Toggles the visibility of the Pause UI Panel
    // Note: Time.timeScale is handled in GameManager
    public void TogglePausePanel(bool isPaused)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
    }

    // --- HP/MP Update Functions ---

    public void UpdatePlayerHP(int currentHP, int maxHP)
    {
        if (playerHPSlider != null)
        {
            playerHPSlider.maxValue = maxHP;
            playerHPSlider.value = currentHP;
        }
    }

    public void UpdatePlayerMP(int currentMP, int maxMP)
    {
        if (playerMPSlider != null)
        {
            playerMPSlider.maxValue = maxMP;
            playerMPSlider.value = currentMP;
        }
    }

    public void UpdateEnemyHP(int currentHP, int maxHP)
    {
        if (enemyHPSlider != null)
        {
            enemyHPSlider.maxValue = maxHP;
            enemyHPSlider.value = currentHP;
        }
    }

    // --- Result Functions ---

    public void ShowResult(string message)
    {
        if (resultPanel != null && resultText != null)
        {
            resultText.text = message;
            resultPanel.SetActive(true); // Show the entire panel

            // Unlock cursor so the player can click buttons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}