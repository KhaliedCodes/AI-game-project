using UnityEngine;

public class Player : MonoBehaviour
{

    public GameObject redBulletPrefab;
    public GameObject blueBulletPrefab;
    private Vector3 enemyPosition;
    private float nextFireTime = 0f;
    private string enemyType = "RedEnemy";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        enemyType = GameManager.selectedEnemyType;
    }

    void fire()
    {
        var enemyDirection = (enemyPosition - transform.position).normalized;
        transform.up = enemyDirection;
        var bullet = Instantiate(enemyType == "RedEnemy" ? redBulletPrefab : blueBulletPrefab, transform.position, Quaternion.identity);
        bullet.transform.up = enemyDirection;


    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(enemyType))
        {
            // Quaternion.LookRotation(direction);
            if (Time.time >= nextFireTime)
            {
                enemyPosition = collision.gameObject.transform.position;
                nextFireTime = Time.time + 5f;
                fire();
            }
            // Destroy(gameObject);
        }

    }

}
