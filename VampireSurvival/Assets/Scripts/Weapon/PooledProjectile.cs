using UnityEngine;

public class PooledProjectile : MonoBehaviour, IPooledObject
{
    private WeaponData data;
    private Vector2 direction;
    private Camera mainCamera;

    public void OnObjectSpawn()
    {
        mainCamera = Camera.main;
    }

    public void OnObjectDespawn()
    {
        data = null;
        direction = Vector2.zero;
        mainCamera = null;
    }

    public void Setup(WeaponData weaponData, Vector2 dir)
    {
        data = weaponData;
        direction = dir.normalized;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (data == null || mainCamera == null) return;

        // Движение
        transform.position += (Vector3)(direction * data.speed * Time.deltaTime);

        if (IsOutOfCameraBounds())
        {
            ReturnToPool();
        }
    }

    private bool IsOutOfCameraBounds()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        return viewportPos.x < -0.2f || viewportPos.x > 1.2f ||
               viewportPos.y < -0.2f || viewportPos.y > 1.2f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && data != null)
        {
            PoolableEnemy enemy = other.GetComponent<PoolableEnemy>();
            if (enemy != null)
                enemy.TakeDamage(data.damage);

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
