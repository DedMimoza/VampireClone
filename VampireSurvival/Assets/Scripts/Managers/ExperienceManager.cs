using System;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    public event Action<float> OnExperienceGained;

    public event Action OnLevelUp;

    private float currentExperience = 0f;
    public int playerLevel = 1;
    private float experienceToNextLevel = 150f;

    private void Awake()
    {
        Instance = this;
    }

    public void AddExperience(float amount)
    {
        currentExperience += amount;
        OnExperienceGained?.Invoke(currentExperience);

        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentExperience -= experienceToNextLevel;
        playerLevel++;
        experienceToNextLevel *= 1.15f;
        Debug.Log("levelUp");
        if ( WeaponLevelManager.Instance != null)
        {
            var weapons = WeaponLevelManager.Instance.GetEquippedWeapons();
            string randomWeapon = weapons[UnityEngine.Random.Range(0, weapons.Count)];
            WeaponLevelManager.Instance.UpgradeWeapon(randomWeapon);
            Debug.Log($"Level {playerLevel}: Прокачан {randomWeapon.ToUpper()}!");
        }

        OnLevelUp?.Invoke();
    }

    public float GetCurrentXP() => currentExperience;

    public float GetXPToNextLevel() => experienceToNextLevel;

    public int GetPlayerLevel() => playerLevel;
}