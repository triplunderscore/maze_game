using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public float baseHealth = 100f;
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public float attackPower = 10f;
    public float defensePower = 5f;

    public int level = 1;
    public float currentXP = 0f;
    public float maxXP = 100f;

    public void ResetForNewGame()
    {
        level = 1;
        attackPower = 10f;
        defensePower = 5f;
        maxHealth = baseHealth;
        currentHealth = maxHealth;
        currentXP = 0f;
        maxXP = 100f;
    }

    public void PrepareForNextLevel()
    {
        maxHealth += 50f;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0f) currentHealth = 0f;
    }

    public void AddXP(float xp)
    {
        currentXP += xp;
        if (currentXP >= maxXP)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentXP = 0f;
        maxXP += 50f;
        PrepareForNextLevel(); // Boost health for next level
    }

    public void BoostAttack()
    {
        attackPower += 5f;
    }

    public void BoostDefense()
    {
        defensePower += 5f;
    }
}
