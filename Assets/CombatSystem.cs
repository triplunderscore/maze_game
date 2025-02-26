using UnityEngine;


public class CombatSystem : MonoBehaviour
{
    public float health = 100f; // Player health
    public float impactDamage = 10f; // Damage to opponent
    public Animator animator; // Reference to Animator

    // Define moves and health costs
    [System.Serializable]
    public struct Move
    {
        public string name; // Move name
        public KeyCode key; // Input key
        public float healthCost; // Cost of move
        public string animationTrigger; // Animation trigger name
        public float impactDamage; // Damage this move deals
    }

    public Move[] moves; // List of moves

    void Update()
    {
        foreach (var move in moves)
        {
            // Check if move key is pressed and player has enough health
            if (Input.GetKeyDown(move.key) && health >= move.healthCost)
            {
                PerformMove(move);
                break;
            }
        }
    }

    void PerformMove(Move move)
    {
        health -= move.healthCost; // Deduct health
        animator.SetTrigger(move.animationTrigger); // Play animation
        Debug.Log($"Performed {move.name}, Impact Damage: {move.impactDamage}");
        // TODO: Apply impact damage to the opponent here
    }
}

