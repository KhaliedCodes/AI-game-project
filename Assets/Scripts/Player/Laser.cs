using UnityEngine;

public class Laser : MonoBehaviour
{


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RedEnemy"))
        {
            Destroy(collision.gameObject);
        }

    }
}
