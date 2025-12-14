using UnityEngine;
using System.Collections;

public class MeleeAI : MonoBehaviour
{
    private Building _buildingScript;
    private float _attackTimer;
    
    [Header("Live Stats")]
    public float currentDamage;
    public float currentAttackSpeed;
    public float currentRange; 

    // Visuals
    private Vector3 _originalPos;
    private bool _isAttacking = false;

    void Start()
    {
        _buildingScript = GetComponent<Building>();
        // Capture local position so we always return to the center of the tile
        _originalPos = transform.localPosition; 
        
        Invoke("ResetStats", 0.1f);
    }

    public void ResetStats()
    {
        if (_buildingScript != null && _buildingScript.data != null)
        {
            currentDamage = _buildingScript.data.damage;
            currentAttackSpeed = _buildingScript.data.attackSpeed;
            currentRange = _buildingScript.data.range;
        }
    }

    public void ApplyBuff(float damageMult, float speedMult)
    {
        currentDamage *= damageMult;
        currentAttackSpeed *= speedMult;
    }

    void Update()
    {
        if (_buildingScript == null || !_buildingScript.isPlaced) return;

        _attackTimer += Time.deltaTime;
        float speed = (currentAttackSpeed > 0) ? currentAttackSpeed : 1f;

        if (_attackTimer >= (1f / speed))
        {
            CheckForEnemyAndAttack();
        }
    }

    void CheckForEnemyAndAttack()
    {
        // 1. RaycastAll to see through ourselves and find the enemy
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right, currentRange);
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                // Found a valid target!
                Attack(hit.collider.gameObject);
                _attackTimer = 0; 
                return; // Stop searching
            }
        }
    }

    void Attack(GameObject target)
    {
        // 1. Deal Damage
        Enemy enemyScript = target.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.TakeDamage(currentDamage);
        }

        // 2. Play Lunge Animation (Visuals)
        if (!_isAttacking)
        {
            // DYNAMIC DISTANCE LOGIC:
            // If Range is small (Sword/2), move a little (0.7f).
            // If Range is big (Spear/4), move a lot (2.0f).
            
            float pokeDist = 0.7f; // Default short poke
            
            if (currentRange > 3f) 
            {
                pokeDist = 2.0f; // Long stab for Spears
            }

            StartCoroutine(LungeAnimation(pokeDist));
        }
    }

    // Now accepts a distance!
    IEnumerator LungeAnimation(float distance)
    {
        _isAttacking = true;
        
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (Vector3.right * distance);

        float speed = 8.0f; // Fast stab speed
        
        // 1. Stab Forward
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            yield return null;
        }

        // 2. Pull Back
        while (Vector3.Distance(transform.position, startPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
            yield return null;
        }

        // Ensure we are exactly back at the center
        transform.position = startPos;
        _isAttacking = false;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * currentRange));
    }
}