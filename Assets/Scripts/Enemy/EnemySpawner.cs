using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject redEnemyPrefab;
    public GameObject blueEnemyPrefab;
    public GameObject player;
    public float spawnRate = 3f;
    private List<Vector3> spawnPoisitons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoisitons = new() {
            new Vector3(-5, 5, 0),
    new Vector3(-2.5f, 5, 0),
    new Vector3(0, 5, 0),
    new Vector3(2.5f, 5, 0),
    new Vector3(5, 5, 0)};
        InvokeRepeating(nameof(SpawnEnemy), 0, spawnRate);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnEnemy()
    {
        var enemyPosition = spawnPoisitons[Random.Range(0, spawnPoisitons.Count)];
        var enemyPrefab = Random.Range(0, 2) == 0 ? redEnemyPrefab : blueEnemyPrefab;
        var enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        enemy.GetComponent<Movement>().target = player.transform;
    }
}
