using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatedController : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float sprintMultiplier = 1.5f;
    [Header("Jump Paramateres")]
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private float horizontalInput;
    private bool isGrounded;
    private bool shouldJump;
    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Start(){
        inputHandler = PlayerInputHandler.Instance;
    }

    void Update(){
        horizontalInput = inputHandler.MoveInput.x;
        shouldJump = inputHandler.JumpTriggered && isGrounded;
        if(horizontalInput!=0){
            FlipSprite(horizontalInput);
        }
    }

    void FixedUpdate(){
        ApplyMovement();
        if(shouldJump) ApplyJump();

    }

    void ApplyMovement(){
        //If sprinting, multiply move speed by the sprintMultiplier. Else, multiply by 1f.
        float speed = moveSpeed * (inputHandler.SprintValue>0 ? sprintMultiplier : 1f); 
        rb.linearVelocity = new Vector2(horizontalInput*speed,rb.linearVelocityY);
    }

    void ApplyJump(){
        rb.AddForce(Vector2.up*jumpForce,ForceMode2D.Impulse);
        isGrounded = false;
        shouldJump = false;
    }

    void FlipSprite(float horizontalMovement){
        if(horizontalMovement<0){
            transform.localScale = new Vector3(1,1,1);
        }
        else if(horizontalMovement>0){
            transform.localScale = new Vector3(-1,1,1);
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Ground")){
            isGrounded = true;
        }
    }
}
