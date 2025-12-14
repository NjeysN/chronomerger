using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    void OnMouseDown()
    {
        // When you click this object, try to upgrade
        ShopManager.Instance.UpgradeAge();
    }
    
    // Optional: Add hover effect
    void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = Color.yellow;
    }
    
    void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}