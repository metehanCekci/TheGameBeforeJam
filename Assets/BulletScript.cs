using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletScript : MonoBehaviour
{
    [Header("Delay of deletion")]
    [SerializeField] private float DeleteDelay;
    public float speed = 2f;
    public int damage = 1;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake() {
    }

    // Update is called once per frame
    void Update()
    {
        if((timer+=Time.deltaTime)>DeleteDelay){
            Destroy(gameObject);
        }
        else{
            transform.position += transform.right * Time.deltaTime * speed;
        }
        
    }
}
