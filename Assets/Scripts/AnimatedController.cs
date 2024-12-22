using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedController : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 1f;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private bool isGrounded;
    private bool shouldJump;
    private Vector2 fallVector;

    [SerializeField] private GameObject deathMenu;

    [Header("Joystick Settings")]
    [SerializeField] private TouchJoystick touchJoystick; // Joystick for touch input
    [SerializeField] private Transform gunTransform; // Gun transform
    [SerializeField] private SprintButton sprintButton;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab; // Bullet prefab
    [SerializeField] private Transform firePoint; // Bullet spawn point
    [SerializeField] private float bulletInterval = 0.25f; // Bullet creation interval
    [SerializeField] private int bulletAmount = 100;
    [SerializeField] private TMP_Text bulletHud;

    private float lastBulletTime = 0f; // Last bullet creation time

    private Quaternion initialGunRotation; // Initial gun rotation
    private bool isGunFlipped; // Gun flip status

    // Invincibility settings
    [SerializeField] private float invincibilityDuration = 2f; // Duration of invincibility after taking damage
    private bool isInvincible = false; // Whether the player is invincible
    private float invincibilityTimer = 0f; // Timer to track invincibility duration

    private Collider2D playerCollider; // Player's collider to control collision during invincibility
    private SpriteRenderer spriteRenderer; // To control sprite transparency

    public GameObject DropText;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>(); // Get the player's collider component
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the player's SpriteRenderer component
    }

    void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
        fallVector = new Vector2(0, -Physics2D.gravity.y);
        initialGunRotation = gunTransform.rotation; // Store initial gun rotation
    }

    void Update()
    {
        // Joystick input
        Vector2 joystickInput = touchJoystick.GetJoystickInput();

        // Horizontal movement control
        float horizontalInput = inputHandler.MoveInput.x;

        // Jump logic
        shouldJump = inputHandler.JumpTriggered && isGrounded;

        bulletHud.text = bulletAmount.ToString();

        // Apply fall speed increase when falling
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity -= fallVector * fallMultiplier * Time.deltaTime;
        }

        // Aim gun based on joystick input
        AimGun(joystickInput);

        // If joystick is held down, continuously spawn bullets
        if (joystickInput.sqrMagnitude > 0.1f)
        {
            if (Time.time - lastBulletTime >= bulletInterval)
            {
                SpawnBullet();
                lastBulletTime = Time.time;
            }
        }

        // Handle invincibility timer
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false; // End invincibility
                EnableEnemyCollision(); // Re-enable collisions with enemies after invincibility ends
                RestorePlayerOpacity(); // Restore original opacity after invincibility ends
            }
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
        if (shouldJump) ApplyJump();
    }

    // Movement function
    void ApplyMovement()
    {
        float horizontalInput = inputHandler.MoveInput.x;

        float speed = moveSpeed ;

        // Apply movement using Rigidbody2D velocity
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
    }

    // Jump function
    [HideInInspector]
    public void ApplyJump()
    {
        if (isGrounded)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        shouldJump = false;
    }

    // Aiming function
    private void AimGun(Vector2 joystickInput)
    {
        if (joystickInput.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(joystickInput.y, joystickInput.x) * Mathf.Rad2Deg;
            gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (joystickInput.x < 0)
            {
                if (!isGunFlipped)
                {
                    gunTransform.gameObject.GetComponent<SpriteRenderer>().flipY = true;
                    isGunFlipped = true;
                }
            }
            else
            {
                if (isGunFlipped)
                {
                    gunTransform.gameObject.GetComponent<SpriteRenderer>().flipY = false;
                    isGunFlipped = false;
                }
            }
        }
    }

    // Bullet spawning function
    private void SpawnBullet()
    {
        if (bulletAmount > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.SetActive(true);
            bullet.transform.localScale = new Vector3(1.8f, 1f, 1f);
            bulletAmount--;
        }
        else
        {
            deathMenu.SetActive(true);
        }
    }

    // Collision detection for damage and invincibility
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("HurtBox") && !isInvincible) // Check if the player is not invincible
        {
            TakeDamage();
            ActivateInvincibility(); // Activate invincibility after taking damage
        }
    }

    // Activates invincibility for the player
    private void ActivateInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration; // Set the timer for invincibility
        DisableEnemyCollision(); // Disable collisions with enemies during invincibility
        MakePlayerTransparent(); // Make the player transparent
    }

    // Make the player transparent (invincibility effect)
    private void MakePlayerTransparent()
    {
        Color tempColor = spriteRenderer.color;
        tempColor.a = 0.5f; // Set the alpha value to 0.5 (transparent)
        spriteRenderer.color = tempColor;
    }

    // Restore the player's opacity after invincibility ends
    private void RestorePlayerOpacity()
    {
        Color tempColor = spriteRenderer.color;
        tempColor.a = 1f; // Restore alpha value to 1 (fully opaque)
        spriteRenderer.color = tempColor;
    }

    // Disable collisions with enemies during invincibility
    private void DisableEnemyCollision()
    {
        // Get all enemy objects in the scene (assuming they have the "Enemy" tag)
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("HurtBox");

        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>(); // Get the enemy's collider
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, enemyCollider, true); // Ignore collision with the player
            }
        }
    }

    // Re-enable collisions with enemies after invincibility ends
    private void EnableEnemyCollision()
    {
        // Get all enemy objects in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("HurtBox");

        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>(); // Get the enemy's collider
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, enemyCollider, false); // Re-enable collision with the player
            }
        }
    }

    public void GainBullet(int bullet)
    {
        bulletAmount += bullet;
        GameObject clone = Instantiate(DropText);
        clone.transform.position = DropText.transform.position;
        clone.transform.parent = DropText.transform.parent;
        clone.SetActive(true);
        clone.GetComponent<TMP_Text>().color = Color.green;
        clone.GetComponent<TMP_Text>().text = "+"+ bullet;
    }

    public void TakeDamage()
    {
        bulletAmount -= 20; // Decrease bullet amount on damage
        GameObject clone = Instantiate(DropText);
        clone.transform.position = DropText.transform.position;
        clone.transform.parent = DropText.transform.parent;
        clone.SetActive(true);
        clone.GetComponent<TMP_Text>().color = Color.red;
        clone.GetComponent<TMP_Text>().text = "-20";
    }
}
