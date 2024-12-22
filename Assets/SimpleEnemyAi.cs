using UnityEngine;

public class SimpleEnemyAi : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;  // The speed at which the enemy moves

    private Transform player; // Reference to the player's Transform
    private Rigidbody2D rb;   // Reference to the Rigidbody2D for movement

    void Start()
    {
        // Get references to the necessary components
        player = GameObject.FindWithTag("Player").transform; // Assuming the player is tagged "Player"
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Always move towards the player
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        // Calculate the direction towards the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Move the enemy using Rigidbody2D
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
    }
}
