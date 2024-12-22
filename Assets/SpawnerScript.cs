using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] enemies;  // Yaratıkların dizisi
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
            if(startingInterval>1.25f)
            startingInterval -= 0.01f;
            spawnInterval = Random.Range(startingInterval-1,startingInterval+1);
        }
    }

    // Yaratık spawn fonksiyonu
    private void SpawnEnemy()
    {
        if (enemies.Length > 0) // Eğer yaratık dizisi boş değilse
        {
            // Diziden rastgele bir yaratık seç
            int randomIndex = Random.Range(0, enemies.Length);
            GameObject enemyToSpawn = enemies[randomIndex];

            // Yaratığın spawn edileceği x pozisyonunu rastgele belirle
            float randomX = Random.Range(spawnRange.x, spawnRange.y);

            // Yeni yaratığı spawn et
            GameObject clone = Instantiate(enemyToSpawn);
            clone.transform.position = new Vector3(transform.position.x,transform.position.y,0);
            
            clone.SetActive(true);
        }
    }
}
