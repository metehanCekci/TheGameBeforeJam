﻿using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class AnimatedController : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] public float moveSpeed = 5f;
    public bool doubleShot = false;
    public int resurrection = 0;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 1f;
    [Header("Exp Properties")]
    [SerializeField] private int expMultiplier = 2;
    [SerializeField] private int expThreshold = 10;
    [Header("Exp Display Texts")]
    [SerializeField] private TMP_Text expCurrentText;
    [SerializeField] private TMP_Text expThreshText;

    public int DamageAmount = 35;
    public int DefenceScale;
    public TimerScript timer;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private bool isGrounded;
    private bool shouldJump;
    private Vector2 fallVector;
    private int expCurrent = 0;

    [SerializeField] private GameObject deathMenu;

    [Header("Joystick Settings")]
    [SerializeField] public TouchJoystick touchJoystick; // Joystick for touch input
    [SerializeField] private Transform gunTransform; // Gun transform
    [SerializeField] private SprintButton sprintButton;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab; // Bullet prefab
    [SerializeField] private Transform firePoint; // Bullet spawn point
    [SerializeField] public float bulletInterval = 0.5f; // Bullet creation interval
    [SerializeField] public int bulletAmount = 100;
    [SerializeField] private TMP_Text bulletHud;

    private float lastBulletTime = 0f; // Last bullet creation time

    private bool isDead = false;

    public Quaternion initialGunRotation; // Initial gun rotation
    public bool isGunFlipped; // Gun flip status
    private GameObject SFXPlayer;

    // Invincibility settings
    [SerializeField] public float invincibilityDuration = 2f; // Duration of invincibility after taking damage
    private bool isInvincible = false; // Whether the player is invincible
    private float invincibilityTimer = 0f; // Timer to track invincibility duration

    private Collider2D playerCollider; // Player's collider to control collision during invincibility
    private SpriteRenderer spriteRenderer; // To control sprite transparency

    public GameObject DropText;

    // Timer for the damage boost
    private float damageBoostTimer = 0f; // Tracks time since last damage boost
    private float damageBoostInterval = 90f; // 2 minutes (120 seconds)

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
        SFXPlayer = GameObject.FindGameObjectWithTag("SFX");
        //expThreshText.text = "/" + expThreshold.ToString();
    }

    void Update()
    {
        initialGunRotation = gunTransform.rotation;

        DefenceScale = Convert.ToInt16(Math.Round((DamageAmount / 100.0) * 10));

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
        if (Input.GetKeyDown(KeyCode.Mouse0) && SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (!GameObject.FindGameObjectWithTag("MobileControlHud") == false)
            {
                if (GameObject.FindGameObjectWithTag("MobileControlHud").activeInHierarchy == true)
                {
                    GameObject.FindGameObjectWithTag("MobileControlHud").SetActive(false);
                }
            }

        }
        else
        {
            AimGun(joystickInput);
        }

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

        // Check if it's time to double the damage
        damageBoostTimer += Time.deltaTime;
        if (damageBoostTimer >= damageBoostInterval)
        {
            damageBoostTimer = 0f; // Reset the timer
            DamageAmount = Mathf.CeilToInt(DamageAmount * 2.5f);
        }
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure z position is 0 for 2D
            Vector3 direction = mousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Silah i�in mouse y�n�
        }
        // Sol mouse t�k bas�ld���nda ate� etme
        if (Input.GetMouseButton(0)) // 0 sol mouse butonunu temsil eder
        {
            if (GameObject.FindGameObjectWithTag("MobileControlHud") == false)
                if (Time.time - lastBulletTime >= bulletInterval)
                {
                    SpawnBullet();
                    lastBulletTime = Time.time;
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

        float speed = moveSpeed;

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
                    gunTransform.localScale = new Vector3(gunTransform.localScale.x, -gunTransform.localScale.y, gunTransform.localScale.z);
                    isGunFlipped = true;
                }
            }
            else
            {
                if (isGunFlipped)
                {
                    gunTransform.localScale = new Vector3(gunTransform.localScale.x, -gunTransform.localScale.y, gunTransform.localScale.z);
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
            // First bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.SetActive(true);
            bullet.transform.localScale = new Vector3(1.8f, 1f, 1f);
            bulletAmount--;
            SFXPlayer.gameObject.GetComponent<SFXScript>().PlayGunShot();

            // If DoubleShot is active, spawn another bullet with a delay
            if (doubleShot)
            {
                StartCoroutine(SpawnSecondBullet());
            }
        }
        else
        {
            if (resurrection > 0)
            {
                UseResurrection();
            }
            else
            {
                if(!isDead)
                {
                deathMenu.SetActive(true);
                timer.StopTime();
                Time.timeScale = 0;
                isDead = true;
                }
            }

        }
    }

    private IEnumerator SpawnSecondBullet()
    {
        yield return new WaitForSeconds(0.1f); // 0.1 second delay
        if (bulletAmount > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.SetActive(true);
            bullet.transform.localScale = new Vector3(1.8f, 1f, 1f);
            bulletAmount--;
            SFXPlayer.gameObject.GetComponent<SFXScript>().PlayGunShot();
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

    void OnCollisionStay2D(Collision2D collision) {


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
        clone.GetComponent<TMP_Text>().text = "+" + bullet;
    }

    public void GainExp()
    {
        // The exp code remains unchanged
    }

    void TakeDamage()
    {
        // Play damage sound
        SFXPlayer.gameObject.GetComponent<SFXScript>().PlayDamage();

        // Reduce bulletAmount based on damage
        bulletAmount -= (DamageAmount - ((DamageAmount / 100) * DefenceScale));

        // Show damage on the screen
        GameObject clone = Instantiate(DropText);
        clone.transform.position = DropText.transform.position;
        clone.transform.parent = DropText.transform.parent;
        clone.SetActive(true);
        clone.GetComponent<TMP_Text>().color = Color.red;
        clone.GetComponent<TMP_Text>().text = DamageAmount.ToString();

        // Check if player has no bullets left
        if (bulletAmount <= 0)
        {
            // If resurrection is available, cancel the death
            if (resurrection > 0)
            {
                UseResurrection();
            }
            else
            {
                if(!isDead)
                {// No resurrection available, show the death menu
                deathMenu.SetActive(true);
                timer.StopTime();
                Time.timeScale = 0;
                isDead = true;
                }
            }
        }
    }
    private void UseResurrection()
    {
        // Cancel the death (reset player stats)
        resurrection--;  // Decrease resurrection count
        bulletAmount = 100; // Restore bullet amount (you can set this to whatever value you want)

        // You can also restore other stats like health here if needed.

        // Optionally, you can add an effect or animation to show the resurrection happening
        // For now, just reactivate the player and hide the death menu
        deathMenu.SetActive(false);
        Time.timeScale = 1;  // Resume time
        Debug.Log("Player resurrected! Remaining resurrections: " + resurrection);
    }
}