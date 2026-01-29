using UnityEngine;

public class GarlicAura : MonoBehaviour, IPooledObject
{
    private WeaponData data;
    private CircleCollider2D col;
    private Transform player;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        player = FindObjectOfType<PlayerController>().transform;
    }

    public void OnObjectSpawn()
    { }

    public void OnObjectDespawn()
    { data = null; }

    public void Setup(WeaponData weaponData)
    {
        data = weaponData;
        if (col != null)
            col.transform.localScale = new Vector3(data.range, data.range, 1f);
    }

    private void Update()
    {
        if (player != null)
            transform.position = player.position;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && data != null)
        {
            PoolableEnemy enemy = other.GetComponent<PoolableEnemy>();
            if (enemy != null)
                enemy.TakeDamage(data.damage * Time.deltaTime * 5f);
        }
    }
}