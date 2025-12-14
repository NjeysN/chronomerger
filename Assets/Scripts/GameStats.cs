using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    [Header("Resources")]
    public int currentGold = 10;
    public int currentHealth = 20; 
    public int maxHealth = 20;

    [Header("UI References")]
    public TextMeshProUGUI goldText;
    public Image healthBarFill; 
    public GameObject gameOverScreen; 
    public GameObject pauseMenuPanel; // <--- NEW: Drag your Panel here!

    private bool _isPaused = false;

    void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    void Update()
    {
        // Optional: Press 'ESC' to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0; // FREEZE TIME
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1; // UNFREEZE TIME
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        }
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateUI();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateUI();
        if (currentHealth <= 0) GameOver();
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 0; 
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit(); // Closes the window in a real build
    }

    void UpdateUI()
    {
        if (goldText != null) goldText.text = "$ " + currentGold;
        
        if (healthBarFill != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = fillAmount;
        }
    }
}