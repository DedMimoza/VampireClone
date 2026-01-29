using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI experienceText;

    public TextMeshProUGUI levelText;

    private void OnEnable()
    {
        ExperienceManager.Instance.OnExperienceGained += UpdateExperienceUI;
        ExperienceManager.Instance.OnLevelUp += UpdateLevelUI;

        UpdateExperienceUI(0);
        UpdateLevelUI();
    }

    private void OnDisable()
    {
        ExperienceManager.Instance.OnExperienceGained -= UpdateExperienceUI;
        ExperienceManager.Instance.OnLevelUp -= UpdateLevelUI;
    }

    private void UpdateExperienceUI(float currentXP)
    {
        if (experienceText != null)
            experienceText.text = $"XP: {currentXP:F0}/{ExperienceManager.Instance.GetXPToNextLevel():F0}";
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = $"Level: {ExperienceManager.Instance.GetPlayerLevel()}";
    }
}