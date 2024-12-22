using UnityEngine;

public class EnemyHealthScript : MonoBehaviour
{
    public int hp = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other) {

                if(other.gameObject.CompareTag("Bullet"))
        {

            hp--;
            if(hp<=0) Destroy(this.gameObject);
            Destroy(other.gameObject);
        }
    }
}
