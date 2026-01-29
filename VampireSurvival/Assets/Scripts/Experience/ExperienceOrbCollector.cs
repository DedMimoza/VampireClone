using UnityEngine;

public class ExperienceOrbCollector : MonoBehaviour, IPooledObject
{
    [HideInInspector] public Vector2 PlayerPosition => player?.position ?? Vector2.zero;
    [HideInInspector] public float MoveSpeed => DataManager.Instance?.experienceConfig?.moveSpeed ?? 3f;
    public bool IsCollected => collected;
    public bool IsAttracting => distance <= attractRadius && !collected;

    private Transform player;
    private float experienceValue;
    private float attractRadius;
    private float collectionRadius;
    private bool collected = false;
    private float distance;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>()?.transform;
        LoadConfig();
    }

    public void OnObjectSpawn()
    {
        collected = false;
        player = FindObjectOfType<PlayerController>()?.transform;
        LoadConfig();
    }

    public void OnObjectDespawn()
    {
        collected = true;
        player = null;
    }

    private void LoadConfig()
    {
        if (DataManager.Instance?.experienceConfig == null) return;
        var config = DataManager.Instance.experienceConfig;
        experienceValue = config.experienceValue;
        attractRadius = config.attractRadius;
        collectionRadius = config.collectionRadius;
    }

    private void Update()
    {
        if (collected || player == null) return;

        distance = Vector2.Distance(transform.position, player.position);

        if (distance <= collectionRadius)
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (collected) return;

        collected = true;

        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperience(experienceValue);
        }

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