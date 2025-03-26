using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public bool enemyInRange = false;
    private EnemyCombat enemy;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemy = other.GetComponent<EnemyCombat>();
            enemyInRange = enemy != null; // Set enemyInRange only if enemy is valid
            Debug.Log(enemy != null ? $"? Enemy found: {enemy.gameObject.name}" : "? Enemy component missing!");
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Exited trigger with: {other.gameObject.name} (Tag: {other.tag})");

        if (other.CompareTag("Enemy"))
        {
            enemyInRange = false;
            enemy = null;
            Debug.Log("Enemy left hitbox!");
        }
    }

    public void ApplyDamage(float damage)
    {
        if (enemyInRange && enemy != null)
        {
            Debug.Log($"Applying {damage} damage to enemy!");
            enemy.TakeDamage(damage);
        }
        else
        {
            Debug.Log("? Enemy not in range, damage not applied!");
        }
    }

}
