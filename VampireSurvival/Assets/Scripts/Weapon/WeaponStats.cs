using UnityEngine;

[System.Serializable]
public class WeaponStats
{
    public float damage, fireRate, speed, range;
    public int projectileCount;
    public bool targeting;

    public static WeaponStats Calculate(string weaponId, int level)
    {
        WeaponStats stats = new();

        if (weaponId == "fireball")
        {
            stats.fireRate = 2f + (level - 1) * 0.5f;
            stats.damage = 50f * (1f + (level - 1) * 0.25f);
            stats.speed = 8f * (1f + (level - 1) * 0.2f);
            stats.projectileCount = Mathf.Min(1 + (level - 1), 4);
            stats.targeting = level >= 4;
        }
        else if (weaponId == "aura")
        {
            stats.fireRate = 0;
            stats.damage = 20f * (1f + (level - 1) * 0.3f);
            stats.range = 1.5f * (1f + (level - 1) * 0.25f);
        }

        return stats;
    }
}