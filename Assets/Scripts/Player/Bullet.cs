using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * 0.01f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RedEnemy") && gameObject.CompareTag("RedBullet"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("BlueEnemy") && gameObject.CompareTag("BlueBullet"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

    }
}
