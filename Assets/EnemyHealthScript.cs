using System.Collections;
using UnityEngine;

public class EnemyHealthScript : MonoBehaviour
{
    public int hp = 2;
    public int bulletReward = 5;
    public int exp = 50;
    private GameObject player;
    private GameObject SFXPlayer;
    private SpriteRenderer spriteRenderer;  // SpriteRenderer bileşeni için bir referans

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();  // SpriteRenderer bileşenini al
        SFXPlayer = GameObject.FindGameObjectWithTag("SFX");
    }

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            hp -= other.gameObject.GetComponent<BulletScript>().damage;

            // Burada renk değişikliği yapılacak
            StartCoroutine(ChangeColorTemporarily());

            Destroy(other.gameObject);

            if (hp <= 0)
            {
                player.GetComponent<AnimatedController>().GainExp();
                player.GetComponent<AnimatedController>().GainBullet(bulletReward);
                SFXPlayer.gameObject.GetComponent<SFXScript>().PlayHit();
                Destroy(this.gameObject);
                Expmanager.Instance.AddExperience(exp);
            }
        }

        else if (other.gameObject.CompareTag("Ball"))
        {
            hp -= other.gameObject.GetComponent<PlanetOrbit>().damage;

            // Burada renk değişikliği yapılacak
            StartCoroutine(ChangeColorTemporarily());

            if (hp <= 0)
            {
                player.GetComponent<AnimatedController>().GainExp();
                player.GetComponent<AnimatedController>().GainBullet(bulletReward);
                Destroy(this.gameObject);
                Expmanager.Instance.AddExperience(exp);
            }
        }
    }

    // Rengini kısa bir süreliğine kırmızıya döndüren coroutine
    private IEnumerator ChangeColorTemporarily()
    {
        // Rengi kırmızıya çevir
        spriteRenderer.color = Color.red;

        // 0.1 saniye bekle
        yield return new WaitForSeconds(0.1f);

        // Rengi eski haline döndür
        spriteRenderer.color = Color.white;
    }
}
