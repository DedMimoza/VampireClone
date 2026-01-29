using UnityEngine;

public class PoolableEnemy : MonoBehaviour, IPooledObject
{
    [HideInInspector] public EnemyData data;
    private float currentHealth;
    private Transform player;

    public System.Action OnEnemyDied;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>()?.transform;
    }

    public void OnObjectSpawn()
    {
        if (data != null)
        {
            currentHealth = data.health;
            gameObject.SetActive(true);
        }
    }

    public void OnObjectDespawn()
    {
        data = null;
        currentHealth = 0f;
        player = null;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            SpawnExperience();
            ReturnToPool(); 
        }
    }

    private void Update()
    {
        if (player == null || data == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * data.speed * Time.deltaTime;
        transform.up = direction;
    }

    void SpawnExperience()
    {
        int orbCount = Mathf.Clamp((int)(data.experience / 5f), 1, 5);
        for (int i = 0; i < orbCount; i++)
        {
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * 0.5f;
            ObjectPoolManager.Instance?.SpawnFromPool("experience", spawnPos, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReturnToPool(); 
        }
    }


    private void ReturnToPool()
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false); 
        }
    }
}
