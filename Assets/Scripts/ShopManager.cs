using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Database")]
    public List<ItemData> allGameItems; 

    [Header("UI References")]
    public ShopSlot[] shopSlots;    
    public GameObject unitPrefab;   
    public GameObject meleePrefab;  
    public GameObject bufferPrefab; 

    [Header("Background & Banner")] 
    public Image backgroundImage;   
    public Sprite[] ageBackgrounds; 
    public Image ageBannerImage;     
    public Sprite[] ageBanners;      

    [Header("Bar Visuals")] 
    public Image healthBarFrame;     
    public Sprite[] healthBarSprites; 
    public Image waveBarFrame;       
    public Sprite[] waveBarSprites;   

    [Header("Shop & Gold Visuals")] 
    public Image shopFrame;          
    public Sprite[] shopFrameSprites;
    
    public Image goldFrame;
    public Sprite[] goldFrameSprites;

    // --- NEW: Audio Settings ---
    [Header("Music")]
    public AudioSource musicSource; 
    public AudioClip[] ageMusic;    
    // ---------------------------

    [Header("Settings")]
    public ItemData startingItem; 
    public int currentAge = 1;
    public int maxAge = 3; 
    public int rerollCost = 10;

    [Header("Debug")]
    public bool isFirstShop = true; 

    private List<ItemData> _currentShopStock = new List<ItemData>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateVisuals(); 
        Invoke("GenerateShopItems", 0.1f);
    }

    void UpdateVisuals()
    {
        int index = currentAge - 1;
        if (index < 0) return;

        // 1. VISUALS (Standard Swapping)
        if (backgroundImage != null && HasSprite(ageBackgrounds, index))
            backgroundImage.sprite = ageBackgrounds[index];

        if (ageBannerImage != null && HasSprite(ageBanners, index))
            ageBannerImage.sprite = ageBanners[index];

        if (healthBarFrame != null && HasSprite(healthBarSprites, index))
            healthBarFrame.sprite = healthBarSprites[index];

        if (waveBarFrame != null && HasSprite(waveBarSprites, index))
            waveBarFrame.sprite = waveBarSprites[index];

        if (shopFrame != null && HasSprite(shopFrameSprites, index))
            shopFrame.sprite = shopFrameSprites[index];

        if (goldFrame != null && HasSprite(goldFrameSprites, index))
            goldFrame.sprite = goldFrameSprites[index];

        // 2. AUDIO (New Logic)
        if (musicSource != null && index < ageMusic.Length)
        {
            AudioClip clipToPlay = ageMusic[index];

            // Only switch if the clip is different (prevents restarting the song every frame)
            if (clipToPlay != null && musicSource.clip != clipToPlay)
            {
                musicSource.clip = clipToPlay;
                musicSource.Play();
            }
        }
    }

    bool HasSprite(Sprite[] list, int index)
    {
        return list != null && index < list.Length && list[index] != null;
    }

    public void OnItemPurchased()
    {
        if (isFirstShop)
        {
            Debug.Log("Tutorial Item Bought! Unlocking full shop.");
            isFirstShop = false; 
            GenerateShopItems(); 
        }
    }

    public void GenerateShopItems()
    {
        _currentShopStock.Clear();
        if (isFirstShop)
        {
            foreach (var slot in shopSlots) if (slot != null) slot.ClearSlot();
            if (shopSlots != null && shopSlots.Length > 0 && startingItem != null)
                shopSlots[0].Setup(startingItem);
            return; 
        }

        for (int i = 0; i < 5; i++) 
        {
            ItemData pickedItem = GetRandomItemBasedOnAge();
            if (shopSlots != null && i < shopSlots.Length && shopSlots[i] != null)
                shopSlots[i].Setup(pickedItem);
        }
    }

    public void UpgradeAge()
    {
        if (currentAge >= maxAge) return;
        currentAge++;
        Debug.Log($"--- UPGRADED TO AGE {currentAge}! ---");
        
        UpdateVisuals(); // This will trigger the new Music and new Images
    }

    public void RerollShop()
    {
        if (GameStats.Instance.SpendGold(rerollCost)) GenerateShopItems();
    }

    private ItemData GetRandomItemBasedOnAge()
    {
        float commonChance = 0; float uncommonChance = 0; float rareChance = 0;

        switch (currentAge)
        {
            case 1: commonChance = 80; uncommonChance = 15; rareChance = 5; break;
            case 2: commonChance = 60; uncommonChance = 30; rareChance = 10; break;
            case 3: commonChance = 40; uncommonChance = 40; rareChance = 20; break;
        }

        float roll = Random.Range(0f, 100f);
        Rarity selectedRarity = Rarity.Common;

        if (roll < commonChance) selectedRarity = Rarity.Common;
        else if (roll < commonChance + uncommonChance) selectedRarity = Rarity.Uncommon;
        else selectedRarity = Rarity.Rare;

        List<ItemData> validItems = new List<ItemData>();
        foreach (ItemData item in allGameItems)
        {
            if (item.rarity == selectedRarity && item.unlockAtAge <= currentAge)
                validItems.Add(item);
        }

        if (validItems.Count > 0) return validItems[Random.Range(0, validItems.Count)];
        if (allGameItems.Count > 0) return allGameItems[Random.Range(0, allGameItems.Count)];

        return null; 
    }
}