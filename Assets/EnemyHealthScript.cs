using UnityEngine;

public class EnemyHealthScript : MonoBehaviour
{
    public int hp = 2;
    public int bulletReward = 5;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void Awake() {
        player = GameObject.FindWithTag("Player");        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other) {

                if(other.gameObject.CompareTag("Bullet"))
        {

            hp--;

            
            Destroy(other.gameObject);
            if(hp<=0) 
            {
                player.GetComponent<AnimatedController>().GainBullet(bulletReward);
                Destroy(this.gameObject);
                
            }

        }
    }
}
