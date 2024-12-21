using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private float horizontalInput;
    private bool isGrounded;
    private bool shouldJump;
    private Vector2 fallVector;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Start(){
        inputHandler = PlayerInputHandler.Instance;
        fallVector = new Vector2(0 , -Physics2D.gravity.y);
    }

    void Update(){
        horizontalInput = inputHandler.MoveInput.x;
        shouldJump = inputHandler.JumpTriggered && isGrounded;

        if(horizontalInput != 0){
            FlipSprite(horizontalInput);
        }

        // Zıplama tuşu bırakıldığında yükselmeyi anında kes
        if (!inputHandler.JumpTriggered && rb.linearVelocity.y > 0) {
            // Y eksenindeki hızı sıfırlıyoruz
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
        }

        if(rb.linearVelocity.y < 0){
            rb.linearVelocity -= fallVector * fallMultiplier * Time.deltaTime;
        }
    }

    void FixedUpdate(){
        ApplyMovement();
        if(shouldJump) ApplyJump();
    }

    void ApplyMovement(){
        // Sprint yapılıyorsa, hareket hızını sprintMultiplier ile çarpıyoruz
        float speed = moveSpeed * (inputHandler.SprintValue > 0 ? sprintMultiplier : 1f); 
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
    }

    void ApplyJump(){
        // Y ekseninde yukarı doğru bir kuvvet ekliyoruz
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        shouldJump = false;
    }

    void FlipSprite(float horizontalMovement){
        if(horizontalMovement < 0){
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(horizontalMovement > 0){
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Ground")){
            isGrounded = true;
        }
    }
}
