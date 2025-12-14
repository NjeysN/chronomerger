using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemName;
    public Sprite icon;
    public Color preferredColor = Color.white;
    [TextArea] public string description; // For the tooltip later

    [Header("Shop Settings")]
    public ItemType itemType;
    public Rarity rarity;
    public int unlockAtAge = 1; // Item won't appear in shop until this Age
    public int goldCost = 50;

    [Header("Combat Stats (Units Only)")]
    public bool canShoot = true;
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float range = 3f;
    public float health = 100f;
    public GameObject projectilePrefab; // Only for ranged units

    [Header("Buffer Stats (Buffers Only)")]
    public bool isBuffer = false;
    public float buffRange = 1.1f; // 1.1 hits adjacent tiles
    public float damageMultiplier = 1.0f; // 1.2 = +20% damage
    public float speedMultiplier = 1.0f;  // 1.5 = +50% speed

    [Header("Merge Logic")]
    // Logic Team: Drag the Item/Material here
    public ItemData validMergePartner; 
    
    // Logic Team: Drag the Result Unit here
    public ItemData resultItem; 
    
    [Header("Branching Logic (Age 3+)")]
    // For T4 -> T5 where you have 2 choices
    public ItemData alternativeMergePartner;
    public ItemData alternativeResultItem;

    public AnimationCurve animationCurve;
}