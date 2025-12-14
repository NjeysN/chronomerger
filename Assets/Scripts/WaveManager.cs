using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public enum GamePhase { Shopping, Combat }

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Settings")]
    public int currentWave = 1;
    public float shoppingDuration = 10f; 
    public float combatDuration = 30f;   
    
    [Header("References")]
    public TextMeshProUGUI waveText; 
    public TextMeshProUGUI timerText; 
    public EnemySpawner spawner;      
    public Image waveProgressBar; 

    [Header("Bar Colors")]
    public Color shoppingColor = Color.yellow;
    public Color combatColor = Color.green;

    public GamePhase currentPhase = GamePhase.Shopping;
    private float _phaseTimer;

    void Awake()
    {
        Instance = this;
        _phaseTimer = shoppingDuration;
        UpdateUI();
    }

    void Update()
    {
        if (currentPhase == GamePhase.Shopping)
        {
            HandleShoppingPhase();
        }
        else if (currentPhase == GamePhase.Combat)
        {
            HandleCombatPhase();
        }
    }

    void HandleShoppingPhase()
    {
        _phaseTimer -= Time.deltaTime;
        
        if (timerText != null) timerText.text = "Next Wave: " + Mathf.CeilToInt(_phaseTimer);

        if (waveProgressBar != null)
        {
            waveProgressBar.color = shoppingColor;
            waveProgressBar.fillAmount = 1f - (_phaseTimer / shoppingDuration); 
        }

        // Show the Banner
        if (ShopManager.Instance.ageBannerImage != null)
            ShopManager.Instance.ageBannerImage.gameObject.SetActive(true);

        if (_phaseTimer <= 0)
        {
            StartWave();
        }
    }

    void HandleCombatPhase()
    {
        _phaseTimer -= Time.deltaTime;

        if (timerText != null) timerText.text = "Survival: " + Mathf.CeilToInt(_phaseTimer);

        if (waveProgressBar != null)
        {
            waveProgressBar.color = combatColor;
            waveProgressBar.fillAmount = 1f - (_phaseTimer / combatDuration); 
        }

        // Show Banner (Optional: you can hide it if you prefer)
        if (ShopManager.Instance.ageBannerImage != null)
            ShopManager.Instance.ageBannerImage.gameObject.SetActive(true);

        if (_phaseTimer <= 0)
        {
            EndWave();
        }
    }

    public void StartWave()
    {
        Debug.Log($"--- STARTING WAVE {currentWave} ---");
        currentPhase = GamePhase.Combat;
        _phaseTimer = combatDuration;

        int enemyCount = 2 + (currentWave * 2); 
        bool isBossWave = (currentWave % 5 == 0); 

        // Tell Spawner to go!
        if (spawner != null)
            spawner.StartSpawning(enemyCount, isBossWave, combatDuration);
            
        UpdateUI();
    }

    public void EndWave()
    {
        Debug.Log("--- WAVE TIME OVER ---");
        
        // --- THIS IS THE MAGIC PART ---
        CheckForAgeUp(); 
        // -----------------------------

        currentPhase = GamePhase.Shopping;
        currentWave++;
        _phaseTimer = shoppingDuration;
        
        // Refresh Shop
        if (ShopManager.Instance != null)
            ShopManager.Instance.GenerateShopItems();
            
        UpdateUI();
    }

    // This function checks if we hit the milestones
    void CheckForAgeUp()
    {
        if (currentWave == 5)
        {
            Debug.Log("COMPLETED WAVE 5 -> MOVING TO AGE 2");
            if (ShopManager.Instance != null) ShopManager.Instance.UpgradeAge();
        }
        else if (currentWave == 10)
        {
            Debug.Log("COMPLETED WAVE 10 -> MOVING TO AGE 3");
            if (ShopManager.Instance != null) ShopManager.Instance.UpgradeAge();
        }
    }

    public void SkipShopTimer()
    {
        if (currentPhase == GamePhase.Shopping) _phaseTimer = 0;
    }

    void UpdateUI()
    {
        if (waveText != null) waveText.text = "Wave " + currentWave;
    }
}