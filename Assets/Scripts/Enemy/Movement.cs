using UnityEngine;

public class Movement : MonoBehaviour
{

    float speed = 0.5f;
    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make enemy face target's direction using Quaternion LookRotation
        Vector3 direction = (target.position - transform.position).normalized;
        transform.up = direction;
        Quaternion.LookRotation(direction);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
}
