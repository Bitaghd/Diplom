using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private SpawnMethod _enemySpawnMethod = SpawnMethod.Random;
    [SerializeField] private int _numberOfEnemiesToSpawn = 5;
    [SerializeField] private float _spawnDelay = 1.0f;
    public List<Enemy> EnemyPrefabs = new List<Enemy>();


    //private ObjectPool<Enemy> _enemyPool;
    private NavMeshTriangulation _triangulation;

    private void Awake()
    {
        //foreach(var enemyType in EnemyPrefabs)
        //{
        //    _enemyPool = new ObjectPool<Enemy>(CreateEnemy, null, null,  null, true, 5, _numberOfEnemiesToSpawn);
        //}
        //for(int i = 0; i < EnemyPrefabs.Count; i++)
        //{
        //    _enemyObjectPools.Add(i, ObjectPool.CreateInstance(EnemyPrefabs[i], _numberOfEnemiesToSpawn));
        //}
    }

    private void Start()
    {
        _triangulation = NavMesh.CalculateTriangulation();
        StartCoroutine(SpawnEnemies());

    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds wait = new WaitForSeconds(_spawnDelay);

        int spawnedEnemies = 0;

        while (spawnedEnemies < _numberOfEnemiesToSpawn)
        {
            if (_enemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(spawnedEnemies);
            }
            else if (_enemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }

            spawnedEnemies++;

            yield return wait;
        }
    }

    private void SpawnRoundRobinEnemy(int spawnedEnemies)
    {
        int spawnIndex = spawnedEnemies % EnemyPrefabs.Count;

        DoSpawnEnemy(spawnIndex);
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, EnemyPrefabs.Count));
    }

    private void DoSpawnEnemy(int spawnIndex)
    {
        Enemy enemy = Instantiate(EnemyPrefabs[spawnIndex]);



        int vertexIndex = Random.Range(0, _triangulation.vertices.Length);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(_triangulation.vertices[vertexIndex], out hit, 2f, -1))
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            agent.Warp(hit.position);
            //enemy.Agent.Warp(hit.position);
            // enemy needs to get enabled and start chasing them
            //enemy.Movement.Player = Player;
            //enemy.Agent.enabled = true;
            //enemy.Movement.StartChasing();
        }
        else
            Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {_triangulation.vertices[vertexIndex]}");
    }


    public enum SpawnMethod
    {
        RoundRobin,
        Random
    }
}
