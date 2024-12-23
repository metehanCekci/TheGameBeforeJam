using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] public GameObject[] enemies;  // Yaratıkların dizisi
    [SerializeField] private float spawnInterval = 5f; // Yaratık çağırma aralığı
                     private float startingInterval;
    [SerializeField] private Vector2 spawnRange = new Vector2(-5f, 5f);  // Yaratıkların spawn olacağı x ekseni aralığı
    [SerializeField] private float spawnHeight = 0f;  // Yaratıkların y eksenindeki pozisyonu

    private float nextSpawnTime = 0f;  // Bir sonraki yaratık spawn zamanı

    private void Start() {
        startingInterval = spawnInterval;
    }

    void Update()
    {
        // Eğer şu anki zaman, bir sonraki spawn zamanından büyükse, yeni yaratık spawn et
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();  // Yaratığı spawn et
            nextSpawnTime = Time.time + spawnInterval;  // Bir sonraki spawn için zamanı ayarla
            if(startingInterval>0.02f)
            startingInterval -= 0.03f;
            if(startingInterval>1.25f)
            spawnInterval = Random.Range(startingInterval-1,startingInterval+1);
        }
    }

    // Yaratık spawn fonksiyonu
    private void SpawnEnemy()
    {
        if (enemies.Length > 0) // Eğer yaratık dizisi boş değilse
        {
            // Diziden rastgele bir yaratık seç
            int randomIndex = Random.Range(0, 12);

            GameObject enemyToSpawn = null;

            if(randomIndex>= 0 && randomIndex<=7)
            {
                enemyToSpawn = enemies[0];
            }
            
            else if(randomIndex>=8 && randomIndex<=10)
            {
                enemyToSpawn = enemies[1];
            }
            
            else if(randomIndex == 11)
            {
                enemyToSpawn = enemies[2];
            }

            // Yaratığın spawn edileceği x pozisyonunu rastgele belirle
            float randomX = Random.Range(spawnRange.x, spawnRange.y);

            // Yeni yaratığı spawn et
            GameObject clone = Instantiate(enemyToSpawn);
            clone.transform.position = new Vector3(transform.position.x,transform.position.y,0);
            
            clone.SetActive(true);
        }
    }
}
