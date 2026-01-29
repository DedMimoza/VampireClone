using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string id, name, prefab;
    public float damage, fireRate, range, speed;
    public int poolSize;
}

[System.Serializable]
public class ExperienceConfig
{
    public string id;
    public float experienceValue, moveSpeed, attractRadius, collectionRadius;
    public int poolSize;
}

[System.Serializable]
public class EnemyData
{
    public string id, prefab;
    public float health, speed, spawnWeight, experience;
    public float minDistance, maxDistance;
    public float size;
    public string color;
}

[System.Serializable]
public class PlayerData
{
    public float moveSpeed = 5f;
    public float acceleration = 15f;
    public float maxHealth = 100f;
    public float invincibleTime = 1.5f;

    public float dashSpeed = 12f;

    public float dashDuration = 0.3f;
}

[System.Serializable]
public class GameConfig
{
    [System.Serializable]
    public class PoolConfig
    { public string tag, prefab; public int size; }

    [System.Serializable]
    public class SpawnSettings
    { public float baseSpawnRate; public int maxEnemies; public float waveMultiplier; }

    public PoolConfig[] pools;
    public SpawnSettings spawnSettings;
}