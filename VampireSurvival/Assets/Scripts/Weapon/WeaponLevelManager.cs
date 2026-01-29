using System.Collections.Generic;
using UnityEngine;

public class WeaponLevelManager : MonoBehaviour
{
    public static WeaponLevelManager Instance;

    [SerializeField] private List<string> equippedWeapons = new() { "aura", "fireball" };
    private Dictionary<string, int> weaponLevels = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            foreach (var weaponId in equippedWeapons)
                weaponLevels[weaponId] = 1;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public int GetWeaponLevel(string weaponId) => weaponLevels.ContainsKey(weaponId) ? weaponLevels[weaponId] : 1;

    public void UpgradeWeapon(string weaponId)
    {
        if (!weaponLevels.ContainsKey(weaponId)) return;
        weaponLevels[weaponId]++;
        Debug.Log($"{weaponId.ToUpper()} УРОВЕНЬ {weaponLevels[weaponId]}!");
    }

    public List<string> GetEquippedWeapons() => equippedWeapons;
}