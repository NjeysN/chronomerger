using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Data (Assigned by Spawner)")]
    public EnemyData data; 

    // Stats (Copied from Data)
    private float _moveSpeed;
    private float _health;
    private float _damage;
    private int _goldReward;

    // Logic Variables
    public float baseXPosition = -8f; 
    [HideInInspector] public EnemySpawner spawner; 
    private Building _combatTarget; 
    private float _attackTimer;
    private bool _isFighting = false;
    private float _attackSpeed = 1f; // Fixed attack speed for now

    // --- SETUP FUNCTION ---
    public void Setup(EnemyData newData)
    {
        data = newData;

        // 1. Apply Stats
        _moveSpeed = data.moveSpeed;
        _health = data.maxHealth;
        _damage = data.damage;
        _goldReward = data.goldReward;

        // 2. Apply Visuals
        GetComponent<SpriteRenderer>().sprite = data.icon;
        
        // Optional: Scale size for Tanks?
        // if (data.enemyName.Contains("Tank")) transform.localScale = Vector3.one * 1.2f;
    }

    void Update()
    {
        if (_isFighting)
        {
            HandleCombat();
            return; 
        }

        MoveForward();
    }

    void MoveForward()
    {
        transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);

        if (transform.position.x <= baseXPosition)
        {
            ReachBase();
        }
    }

    // ... (Combat Logic remains the same as before) ...
    
    public void TakeDamage(float amount)
    {
        _health -= amount;
        if (_health <= 0) Die();
    }

    void Die()
    {
        GameStats.Instance.AddGold(_goldReward);
        Destroy(gameObject);
    }

    void ReachBase()
    {
        GameStats.Instance.TakeDamage((int)_damage);
        Destroy(gameObject);
    }

    void HandleCombat()
    {
        if (_combatTarget == null)
        {
            _isFighting = false;
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= (1f / _attackSpeed))
        {
            if (_combatTarget != null) _combatTarget.TakeDamage(_damage);
            _attackTimer = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Building unit = other.GetComponent<Building>();
        if (unit != null)
        {
            _combatTarget = unit;
            _isFighting = true;
            _attackTimer = 0; 
        }
    }
}