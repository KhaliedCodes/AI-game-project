using UnityEngine;

public class Shield : MonoBehaviour
{


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BlueEnemy"))
        {
            Destroy(collision.gameObject);
        }
    }
}
