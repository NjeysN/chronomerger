using UnityEngine;
using UnityEngine.UI; 
using TMPro;          

public class ShopSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;       
    public TextMeshProUGUI priceText; 
    public GameObject soldOutDimmer;  

    private ItemData _currentItem; 

    public void Setup(ItemData item)
    {
        _currentItem = item;

        if (_currentItem != null)
        {
            iconImage.sprite = _currentItem.icon;
            iconImage.enabled = true;
            priceText.text = "$" + _currentItem.goldCost;
            if (soldOutDimmer != null) soldOutDimmer.SetActive(false);
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        _currentItem = null;
        iconImage.enabled = false; 
        priceText.text = "";
        if (soldOutDimmer != null) soldOutDimmer.SetActive(true); 
    }

    public void OnClick()
    {
        if (_currentItem == null) return;

        if (GameStats.Instance.SpendGold(_currentItem.goldCost))
        {
            SpawnItem();
            ClearSlot(); 
            ShopManager.Instance.OnItemPurchased(); 
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    void SpawnItem()
    {
        // 1. Get Mouse Position
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPos.z = 0;

        // 2. Decide which Prefab to use based on Data
        // Default to Ranger (GenericUnit)
        GameObject prefabToUse = ShopManager.Instance.unitPrefab; 

        if (_currentItem.isBuffer)
        {
            // Case A: It's a Drum/Banner
            prefabToUse = ShopManager.Instance.bufferPrefab;
        }
        else if (_currentItem.projectilePrefab == null) 
        {
             // Case B: NO BULLET? It must be Melee! (Sword or Spear)
             // This works regardless of range (2, 4, 10, etc.)
             prefabToUse = ShopManager.Instance.meleePrefab;
        }
        else
        {
             // Case C: HAS BULLET? It is Ranged!
             prefabToUse = ShopManager.Instance.unitPrefab;
        }

        // 3. Spawn It
        GameObject newItemObj = Instantiate(prefabToUse, spawnPos, Quaternion.identity);

        // 4. Initialize Building Data
        Building building = newItemObj.GetComponent<Building>();
        if (building != null)
        {
            building.data = _currentItem;
            building.isPlaced = false; 
        }
    }
}