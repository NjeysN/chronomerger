using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject enemyPrefab; // Drag the GenericEnemy prefab here
    public Transform[] spawnPoints;

    [Header("Enemy Waves (Drag Data Here)")]
    public EnemyData[] age1Enemies; // Standard, Fast, Tank (Age 1)
    public EnemyData[] age2Enemies; // Standard, Fast, Tank (Age 2)
    public EnemyData[] age3Enemies; // Standard, Fast, Tank (Age 3)

    private float _spawnTimer;
    private int _enemiesRemainingToSpawn;
    private bool _isSpawning = false;

    public void StartSpawning(int count, bool isBoss, float duration)
    {
        _enemiesRemainingToSpawn = count;
        _isSpawning = true;
        
        // Calculate delay between spawns
        _spawnTimer = duration / count;
        
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (_enemiesRemainingToSpawn > 0)
        {
            SpawnEnemy();
            _enemiesRemainingToSpawn--;
            yield return new WaitForSeconds(_spawnTimer);
        }
        _isSpawning = false;
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        // 1. Pick a random spawn point (Lane)
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 2. Determine which enemy to spawn based on Age
        EnemyData dataToSpawn = GetRandomEnemyForCurrentAge();

        if (dataToSpawn == null) return;

        // 3. Create the Generic Enemy
        GameObject newEnemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        
        // 4. Initialize it with Data
        Enemy enemyScript = newEnemyObj.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.Setup(dataToSpawn);
            enemyScript.spawner = this;
        }
    }

    EnemyData GetRandomEnemyForCurrentAge()
    {
        int currentAge = ShopManager.Instance.currentAge;
        EnemyData[] currentPool = age1Enemies; // Default

        if (currentAge == 2) currentPool = age2Enemies;
        else if (currentAge == 3) currentPool = age3Enemies;

        if (currentPool.Length > 0)
        {
            return currentPool[Random.Range(0, currentPool.Length)];
        }
        return null;
    }
}