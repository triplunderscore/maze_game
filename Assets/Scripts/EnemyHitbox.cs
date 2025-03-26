using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHitbox : MonoBehaviour
{
    private bool playerInRange = false;
    private CombatSystem player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<CombatSystem>();
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    public void ApplyDamage(float damage)
    {
        if (playerInRange && player != null)
        {
            player.TakeDamage(damage);
            Debug.Log("Player hit!");
        }
    }
}
