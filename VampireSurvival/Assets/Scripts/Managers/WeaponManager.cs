using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<string> equippedWeapons = new() { "aura", "fireball" };
    private Dictionary<string, float> fireTimers = new();
    private Transform player;
    private bool poolsReady = false;
    private GameObject auraInstance;
    private Vector2 lastMoveDirection = Vector2.right;

    private void OnEnable()
    {
        PlayerController.OnPlayerVelocityChanged += UpdateMoveDirection;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerVelocityChanged -= UpdateMoveDirection;
    }

    private void Awake()
    {
        player = transform;
        foreach (var id in equippedWeapons)
            fireTimers[id] = 0f;
    }

    private void Start()
    {
        Invoke(nameof(CheckPoolsReady), 0.1f);
    }

    private void CheckPoolsReady()
    {
        poolsReady = ObjectPoolManager.Instance != null;
        if (!poolsReady)
            Invoke(nameof(CheckPoolsReady), 0.1f);
        else
            Debug.Log(" WeaponManager: Пулы готовы!");
    }

    private float auraTimer = 0f;
    private const float AURA_CYCLE_TIME = 3f;
    private bool auraActive = false;

    private void Update()
    {
        if (!poolsReady || WeaponLevelManager.Instance == null) return;

        foreach (var weaponId in equippedWeapons)
        {
            WeaponStats stats = WeaponStats.Calculate(weaponId, WeaponLevelManager.Instance.GetWeaponLevel(weaponId));

            if (weaponId == "fireball")
            {
                fireTimers[weaponId] += Time.deltaTime;
                if (stats.fireRate > 0 && fireTimers[weaponId] >= 1f / stats.fireRate)
                {
                    FireProjectile(weaponId, stats);
                    fireTimers[weaponId] = 0f;
                }
            }
            else if (weaponId == "aura")
            {
                auraTimer += Time.deltaTime;
                bool shouldBeActive = auraTimer % AURA_CYCLE_TIME < 1f;

                if (shouldBeActive != auraActive)
                {
                    auraActive = shouldBeActive;
                    ToggleAura(auraActive, stats);
                }
            }
        }
    }

    private void ToggleAura(bool active, WeaponStats stats)
    {
        if (auraInstance == null)
        {
            auraInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/GarlicAura"));
            auraInstance.transform.SetParent(transform);
        }

        auraInstance.SetActive(active);
        auraInstance.transform.position = transform.position;
        auraInstance.transform.localScale = new Vector3(stats.range, stats.range, 1f);

        GarlicAura aura = auraInstance.GetComponent<GarlicAura>();
        if (aura != null)
            aura.Setup(new WeaponData { damage = stats.damage, range = stats.range });
    }

    private void FireProjectile(string weaponId, WeaponStats stats)
    {
        Vector2 direction = stats.targeting ? FindNearestEnemy() : lastMoveDirection;

        for (int i = 0; i < stats.projectileCount; i++)
        {
            Vector2 spreadDir = Quaternion.Euler(0, 0, (i - (stats.projectileCount - 1) / 2f) * 15f) * direction;

            GameObject proj = ObjectPoolManager.Instance.SpawnFromPool(weaponId, player.position, Quaternion.identity);
            if (proj == null) return;

            PooledProjectile projectile = proj.GetComponent<PooledProjectile>();
            if (projectile != null)
                projectile.Setup(new WeaponData { damage = stats.damage, speed = stats.speed }, spreadDir);
        }
    }

    private Vector2 FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f);
        Collider2D nearest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }
        }
        return nearest != null ? (nearest.transform.position - transform.position).normalized : Vector2.right;
    }

    private void UpdateMoveDirection(Vector2 direction)
    {
        lastMoveDirection = direction;
    }

    //private Vector2 GetMoveDirection()
    //{
    //    PlayerController playerController = FindObjectOfType<PlayerController>();
    //    if (playerController == null) return Vector2.right;

    //    Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
    //    Vector2 velocity = rb.velocity.normalized;

    //    return velocity.magnitude > 0.1f ? velocity : lastDirection;
    //}

    private Vector2 lastDirection = Vector2.right;
}