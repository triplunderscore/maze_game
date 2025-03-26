using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public Slider healthBar;
    public Image healthBarFill;
    public Slider xpBar;
    public TMP_Text levelText;
    public TMP_Text attackText;
    public TMP_Text defenseText;

    public PlayerStats stats; // Assign this ScriptableObject in Inspector

    private CombatSystem playerCombat;

    void Start()
    {
        playerCombat = FindObjectOfType<CombatSystem>();
        UpdateStatsUI();
    }

    void Update()
    {
        UpdateHealthUI();
        UpdateXPUI();
        UpdateStatsUI();

        if (AreAllEnemiesDefeated())
        {
            LoadUpgradeScene();
        }
    }

    void UpdateHealthUI()
    {
        healthBar.value = stats.currentHealth / stats.maxHealth;
        UpdateHealthBarColor(stats.currentHealth);
    }

    void UpdateXPUI()
    {
        xpBar.value = stats.currentXP / stats.maxXP;
        levelText.text = "Level " + stats.level;
    }

    public void UpdateStatsUI()
    {
        attackText.text = "Attack: " + stats.attackPower;
        defenseText.text = "Defense: " + stats.defensePower;
    }

    public void OnPlayerHit()
    {
        StartCoroutine(FlashRed());
    }

    void UpdateHealthBarColor(float health)
    {
        float healthPercentage = health / stats.maxHealth;

        if (healthPercentage > 0.7f)
            healthBarFill.color = Color.green;
        else if (healthPercentage > 0.1f && healthPercentage < 0.5f)
            healthBarFill.color = Color.yellow;
        else if (healthPercentage <= 0.1f)
            healthBarFill.color = Color.red;
    }

    IEnumerator FlashRed()
    {
        Color originalColor = healthBarFill.color;
        healthBarFill.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        UpdateHealthBarColor(stats.currentHealth);
    }

    public void AddXP(float xp)
    {
        stats.AddXP(xp);  // Handles level-up internally
        UpdateXPUI();

        if (stats.currentXP == 0f) // Means level-up happened
        {
            LoadUpgradeScene();
        }
    }

    bool AreAllEnemiesDefeated()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }

    void LoadUpgradeScene()
    {
        SceneManager.LoadScene("UpgradeScene");
    }
}
