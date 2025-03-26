using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public float health = 100f;
    public int level = 1;
    public Animator animator;
    public float attackCooldown = 2f;
    public enemyHitbox hitbox;

    [System.Serializable]
    public struct Attack
    {
        public string name;
        public float damage;
        public string animationTrigger;
    }

    public Attack[] attacks;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (!hitbox) hitbox = GetComponentInChildren<enemyHitbox>();
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (health > 0)
        {
            yield return new WaitForSeconds(attackCooldown);
            if (player != null)
            {
                PerformAttack();
            }
        }
    }

    void PerformAttack()
    {
        if (attacks.Length == 0 || player == null) return;

        int index = Random.Range(0, attacks.Length);
        Attack attack = attacks[index];

        animator.SetTrigger(attack.animationTrigger);
        StartCoroutine(ApplyDamageAfterDelay(attack.damage, 0.3f));
    }

    IEnumerator ApplyDamageAfterDelay(float damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        hitbox.ApplyDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {health}");
        if (health <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Enemy defeated!");
        animator.SetTrigger("Die");
        Destroy(gameObject, 1f);
    }
}
