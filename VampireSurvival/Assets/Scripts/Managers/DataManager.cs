using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    // WRAPPERS для массивов JSON
    [System.Serializable] private class WeaponListWrapper { public WeaponData[] weapons; }

    [System.Serializable] private class EnemyListWrapper { public EnemyData[] enemies; }

    [System.Serializable] public class ExperienceWrapper { public ExperienceConfig orb; }

    public Dictionary<string, WeaponData> weaponsDict = new();
    public Dictionary<string, EnemyData> enemiesDict = new();
    public PlayerData playerData = new PlayerData();
    public GameConfig gameConfig = new GameConfig();
    public ExperienceConfig experienceConfig = new ExperienceConfig();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            LoadAllData();
            LoadGameConfig();
        }
        else Destroy(gameObject);
    }

    private void LoadAllData()
    {
        LoadWeapons();
        LoadEnemies();
        LoadPlayer();
        LoadExperience();
        Debug.Log($"LOADED: {weaponsDict.Count} weapons, {enemiesDict.Count} enemies");
    }

    private void LoadGameConfig()
    {
        TextAsset json = Resources.Load<TextAsset>("Data/game");
        if (json != null)
            gameConfig = JsonUtility.FromJson<GameConfig>(json.text);
    }

    private void LoadWeapons()
    {
        TextAsset json = Resources.Load<TextAsset>("Data/weapons");
        if (json == null)
        {
            Debug.LogWarning("weapons.json не найден");
            return;
        }

        var wrapper = JsonUtility.FromJson<WeaponListWrapper>(json.text);
        if (wrapper?.weapons == null)
        {
            Debug.LogError("weapons.json: неверный формат!");
            return;
        }

        foreach (var w in wrapper.weapons)
            if (!string.IsNullOrEmpty(w.id))
                weaponsDict[w.id] = w;
    }

    private void LoadEnemies()
    {
        TextAsset json = Resources.Load<TextAsset>("Data/enemies");
        if (json == null)
        {
            Debug.LogError("enemies.json НЕ НАЙДЕН!");
            return;
        }

        var wrapper = JsonUtility.FromJson<EnemyListWrapper>(json.text);
        if (wrapper?.enemies == null)
        {
            Debug.LogError("enemies.json: неверный формат!");
            return;
        }

        foreach (var e in wrapper.enemies)
            if (!string.IsNullOrEmpty(e.id))
                enemiesDict[e.id] = e;
    }


    void LoadExperience()
    {
        TextAsset json = Resources.Load<TextAsset>("Data/experience");
        if (json != null)
            experienceConfig = JsonUtility.FromJson<ExperienceWrapper>(json.text).orb;
    }

    private void LoadPlayer()
    {
        TextAsset json = Resources.Load<TextAsset>("Data/player");
        if (json != null)
            playerData = JsonUtility.FromJson<PlayerData>(json.text);
    }

    public WeaponData GetWeapon(string id) => weaponsDict.TryGetValue(id, out var w) ? w : null;

    public EnemyData GetEnemy(string id) => enemiesDict.TryGetValue(id, out var e) ? e : null;
}