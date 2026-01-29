using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Spawn")]
    public float spawnRate = 1.5f;
    public int maxEnemies = 100;

    private Camera cam;
    private List<EnemyData> enemyPool;
    private float spawnTimer;
    private List<string> activeEnemyTags = new();

    private void Awake()
    {
        cam = Camera.main;
        Invoke(nameof(DelayedInit), 0.3f);
    }

    private void DelayedInit()
    {
        UpdateSpawnRateFromLevel(); 
        maxEnemies = DataManager.Instance.gameConfig.spawnSettings.maxEnemies;
        BuildEnemyPool();
    }

    private void UpdateSpawnRateFromLevel()
    {
        int currentLevel = ExperienceManager.Instance?.playerLevel ?? 1;

        spawnRate = Mathf.Max(0.3f, 1f - (currentLevel - 1) * 0.075f);

        Debug.Log($"Уровень {currentLevel} → SpawnRate: {spawnRate:F2} сек");
    }

    private void Update()
    {
        UpdateSpawnRateFromLevel();

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate && activeEnemyTags.Count < maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void BuildEnemyPool()
    {
        enemyPool = new List<EnemyData>();
        foreach (var enemy in DataManager.Instance.enemiesDict.Values)
            for (int i = 0; i < enemy.spawnWeight * 10f; i++)
                enemyPool.Add(enemy);
    }

    private void SpawnEnemy()
    {
        EnemyData enemyData = enemyPool[Random.Range(0, enemyPool.Count)];
        Vector2 spawnPos = GetSpawnPosition(enemyData);

        GameObject enemyObj = ObjectPoolManager.Instance.SpawnFromPool(
            enemyData.id, spawnPos, Quaternion.identity
        );

        if (enemyObj != null)
        {
            PoolableEnemy enemy = enemyObj.GetComponent<PoolableEnemy>();
            enemy.data = enemyData;
            activeEnemyTags.Add(enemyData.id);

            enemy.OnEnemyDied += () => activeEnemyTags.Remove(enemyData.id);
        }
    }


    private Vector2 GetSpawnPosition(EnemyData data)
    {
        Vector3 camPos = cam.transform.position;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(data.minDistance, data.maxDistance);

        return (Vector2)camPos + new Vector2(
            Mathf.Cos(angle) * distance,
            Mathf.Sin(angle) * distance
        );
    }

    public void OnLevelChanged()
    {
        UpdateSpawnRateFromLevel();
    }
}
