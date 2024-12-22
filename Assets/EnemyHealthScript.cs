using UnityEngine;

public class EnemyHealthScript : MonoBehaviour
{
    public int hp = 2;
    public int bulletReward = 5;
    private GameObject player;

    void Start()
    {

    }
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }
    int exp = 100;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            hp--;

            Destroy(other.gameObject);
            if (hp <= 0)
            {
                player.GetComponent<AnimatedController>().GainExp();
                player.GetComponent<AnimatedController>().GainBullet(bulletReward);
                Destroy(this.gameObject);
                Expmanager.Instance.AddExperience(exp);
            }
        }
    }
}
