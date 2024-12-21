using TMPro;
using UnityEngine;

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

    [Header("Joystick Settings")]
    [SerializeField] private TouchJoystick touchJoystick; // Dokunmatik joystick
    [SerializeField] private Transform gunTransform; // Silahın Transform'u

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab; // Bullet prefab'ı
    [SerializeField] private Transform firePoint; // Silahın mermi fırlatma noktası
    [SerializeField] private float bulletInterval = 0.25f; // Bullet oluşturma aralığı

    private float lastBulletTime = 0f; // Son oluşturulan merminin zamanı

    private Quaternion initialGunRotation; // Silahın başlangıçtaki rotası
    private bool isGunFlipped; // Silahın flip durumu

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
        fallVector = new Vector2(0, -Physics2D.gravity.y);
        initialGunRotation = gunTransform.rotation; // Silahın başlangıç rotasını kaydediyoruz
    }

    void Update()
    {
        // Joystick input'u al
        Vector2 joystickInput = touchJoystick.GetJoystickInput();

        // Yatay hareket kontrolü
        float horizontalInput = inputHandler.MoveInput.x;

        // Zıplama işlemi
        shouldJump = inputHandler.JumpTriggered && isGrounded;

        // Y eksenindeki hız, düşüşü hızlandırmak için
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity -= fallVector * fallMultiplier * Time.deltaTime;
        }

        // Silahı yönlendirme
        AimGun(joystickInput);

        // Joystick basılıysa sürekli bullet yaratma
        if (joystickInput.sqrMagnitude > 0.1f)
        {
            if (Time.time - lastBulletTime >= bulletInterval)
            {
                SpawnBullet();
                lastBulletTime = Time.time; // Son bullet zamanını güncelle
            }
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
        if (shouldJump) ApplyJump();
    }

    // Hareket fonksiyonu
    void ApplyMovement()
    {
        float horizontalInput = inputHandler.MoveInput.x;
        float speed = moveSpeed * (inputHandler.SprintValue > 0 ? sprintMultiplier : 1f);

        // Rigidbody2D linearVelocity ile hareket sağlama
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
    }

    // Zıplama fonksiyonu
    void ApplyJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        shouldJump = false;
    }

    // Silahı joystick input'una göre yönlendirme
    private void AimGun(Vector2 joystickInput)
    {
        // Joystick input'u çok küçükse silahın dönüşünü sıfırla
        if (joystickInput.sqrMagnitude > 0.1f)
        {
            // Joystick'e göre bir açı hesapla
            float angle = Mathf.Atan2(joystickInput.y, joystickInput.x) * Mathf.Rad2Deg;

            // Silahı döndür
            gunTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Joystick sol tarafta mı kontrol et
            if (joystickInput.x < 0)
            {
                // Silahı Y ekseninde flip et (ters çevir)
                if (!isGunFlipped)
                {
                    gunTransform.gameObject.GetComponent<SpriteRenderer>().flipY = true;
                    isGunFlipped = true; // Flip yapıldığı bilgisini kaydediyoruz
                }
            }
            else
            {
                // Joystick sağ tarafta, normal pozisyona getir
                if (isGunFlipped)
                {
                    gunTransform.gameObject.GetComponent<SpriteRenderer>().flipY = false;
                    isGunFlipped = false; // Flip geri alınıyor
                }
            }
        }
    }

    // Bullet oluşturma fonksiyonu
    private void SpawnBullet()
    {
    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    bullet.SetActive(true);

    bullet.transform.localScale = new Vector3(2f, 2f, 1f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
