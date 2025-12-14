using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "TowerDefense/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Visuals")]
    public string enemyName;
    public Sprite icon;       // The sprite for the enemy

    [Header("Stats")]
    public float moveSpeed;   // Fast = 4, Tank = 1, Normal = 2
    public float maxHealth;   // Fast = 20, Tank = 100, Normal = 50
    public float damage;      // Damage dealt to base/units
    public int goldReward;    // Gold given on death
}