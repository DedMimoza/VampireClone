using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [SerializeField] private List<Pool> pools = new();
    private Dictionary<string, Queue<GameObject>> poolDictionary = new();

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    private void Awake()
    {
        Instance = this;

        Invoke(nameof(InitializePoolsFromConfig), 0.2f);
    }

    private void InitializePoolsFromConfig()
    {
        if (DataManager.Instance?.gameConfig?.pools == null) return;

        foreach (var configPool in DataManager.Instance.gameConfig.pools)
        {
            GameObject prefab = Resources.Load<GameObject>(configPool.prefab);
            if (prefab == null)
            {
                Debug.LogError($"Префаб не найден: {configPool.prefab}");
                continue;
            }

            // Создаем пул автоматически!
            Pool pool = new Pool { tag = configPool.tag, prefab = prefab, size = configPool.size };
            pools.Add(pool);
        }

        InitializePools();
        Debug.Log($"Pools loaded from JSON: {pools.Count}");
    }

    private void InitializePools()
    {
        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"Pool с тегом {tag} не найден!");
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
            pooledObj.OnObjectSpawn();

        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public void ReturnToPool(GameObject obj)
    {
        IPooledObject pooledObj = obj.GetComponent<IPooledObject>();
        pooledObj?.OnObjectDespawn();

        obj.SetActive(false);

        string tag = obj.tag;
        if (poolDictionary.ContainsKey(tag))
        {
            poolDictionary[tag].Enqueue(obj);
            Debug.Log($"{obj.name} возвращен в пул '{tag}'");
        }
    }

}