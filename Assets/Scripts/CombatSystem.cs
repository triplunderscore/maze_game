using UnityEngine;
using System.Collections;


public class CombatSystem : MonoBehaviour
{
    public PlayerStats stats; // Assign the ScriptableObject asset
    public Animator animator;
    public PlayerHitbox hitbox;

    [System.Serializable]
    public struct Move
    {
        public string name;
        public KeyCode key;
        public float healthCost;
        public string animationTrigger;
        public float impactDamage;
    }

    public Move[] moves;

    void Update()
    {
        foreach (var move in moves)
        {
            if (Input.GetKeyDown(move.key) && stats.currentHealth >= move.healthCost)
            {
                PerformMove(move);
                break;
            }
        }
    }

    void PerformMove(Move move)
    {
        stats.currentHealth -= move.healthCost;
        animator.SetTrigger(move.animationTrigger);
        StartCoroutine(ApplyDamageAfterDelay(move.impactDamage + stats.attackPower, 0.3f));
    }

    IEnumerator ApplyDamageAfterDelay(float damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        hitbox.ApplyDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        stats.TakeDamage(damage);
        if (stats.currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Player defeated!");
        Destroy(gameObject);
    }
}

