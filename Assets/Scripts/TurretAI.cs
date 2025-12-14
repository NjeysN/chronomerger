using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [Header("Settings")]
    public GameObject bulletPrefab; 
    public Transform firePoint;     

    private Building _buildingScript;
    private float _fireTimer;
    private Transform _target; 

    [Header("Live Stats")]
    public float currentDamage;
    public float currentAttackSpeed;
    public float currentRange;

    void Start()
    {
        _buildingScript = GetComponent<Building>();
        // Add a small delay to ensure Data is assigned before we read it
        Invoke("ResetStats", 0.1f); 
    }

    public void ResetStats()
    {
        if (_buildingScript != null && _buildingScript.data != null)
        {
            currentDamage = _buildingScript.data.damage;
            currentAttackSpeed = _buildingScript.data.attackSpeed;
            currentRange = _buildingScript.data.range;

            // --- THE MISSING LINK ---
            // Load the bullet from the Item Data into the Turret
            if (_buildingScript.data.projectilePrefab != null)
            {
                bulletPrefab = _buildingScript.data.projectilePrefab;
            }
            // ------------------------

            Debug.Log($"{gameObject.name} Stats Loaded! Range: {currentRange}, Speed: {currentAttackSpeed}");
        }
        else
        {
            Debug.LogError($"{gameObject.name} has NO DATA! Cannot shoot.");
        }
    }

    // --- THIS IS THE FUNCTION THAT WAS MISSING ---
    public void ApplyBuff(float damageMult, float speedMult)
    {
        currentDamage *= damageMult;
        currentAttackSpeed *= speedMult;
        Debug.Log($"{gameObject.name} was buffed!");
    }
    // ---------------------------------------------

    void Update()
    {
        // 1. Safety Checks
        if (_buildingScript == null || !_buildingScript.isPlaced) return;
        if (_buildingScript.data != null && !_buildingScript.data.canShoot) return;

        // 2. Look for enemies
        FindTarget();

        // 3. Shoot if we have a target
        if (_target != null)
        {
            _fireTimer += Time.deltaTime;
            
            // Prevent division by zero error
            float speed = (currentAttackSpeed > 0) ? currentAttackSpeed : 1f;

            if (_fireTimer >= (1f / speed))
            {
                Shoot();
                _fireTimer = 0;
            }
        }
    }

    void FindTarget()
    {
        // Scan for objects inside the range circle
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange);
        
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            // Only care about things tagged "Enemy"
            if (hit.CompareTag("Enemy"))
            {
                // HALF CIRCLE CHECK: Is the enemy to my RIGHT?
                if (hit.transform.position.x >= transform.position.x)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestEnemy = hit.gameObject;
                    }
                }
            }
        }

        if (nearestEnemy != null)
        {
            _target = nearestEnemy.transform;
            // Uncomment this if you want to see exactly when it locks on:
            // Debug.Log($"Target Acquired: {_target.name}");
        }
        else
        {
            _target = null;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) 
        {
            Debug.LogError("Cannot Shoot: Bullet Prefab is Missing inside the Item Data!");
            return;
        }

        if (_target != null)
        {
            // Calculate aim direction
            Vector3 direction = (_target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);

            // Spawn the bullet
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, bulletRotation);
            
            // Set damage
            Projectile proj = bulletObj.GetComponent<Projectile>();
            if (proj != null) proj.damage = currentDamage; 
            
            Debug.Log("Pew!"); // Confirm shooting happens
        }
    }
    
    // Draw the range circle in the Scene View
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, currentRange);
    }
}