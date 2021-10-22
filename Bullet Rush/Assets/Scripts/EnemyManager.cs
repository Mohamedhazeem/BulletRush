using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// Enemy types
    /// spawn enemies
    /// Enemy Die
    /// </summary>
    // Start is called before the first frame update

    public static EnemyManager instance;

    [Header("Enemy Spawn Points")]
    [SerializeField] EnemySpawnPoints[] enemySpawnPoints;

    [Header("Enemy Types")]
    [SerializeField] GameObject[] enemyPrefab;
    [Header("Enemy Spawn Position On Y")]
    [SerializeField] float enemySpawnPositionOnY;

    public List<Transform> enemyList;

    private GameObject enemyGameobject;
    private Vector3 enemyPositionOnSpawnPoint = new Vector3();

    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        
    }

    void Start()
    {
        InstantiateEnemies();
    }
    void Update()
    {
        
    }

    private void InstantiateEnemies()
    {
        foreach (EnemySpawnPoints enemySpawnPoint in enemySpawnPoints)
        {
            for (int i = 0; i < enemySpawnPoint.xAxisRight; i++) //coloumn
            {
             
                for (int j = 0; j < enemySpawnPoint.zAxisDown; j++) // row
                {
                    enemyGameobject = ObjectPoolManager.instance.GetObjectFromPool(enemyPrefab[0]);
                    enemyGameobject.transform.parent = enemySpawnPoint.spawnPoint.transform;
                    enemyPositionOnSpawnPoint.x = enemySpawnPoint.spawnPoint.position.x + i * 1.5f;
                    enemyPositionOnSpawnPoint.y = enemySpawnPositionOnY;
                    enemyPositionOnSpawnPoint.z = enemySpawnPoint.spawnPoint.position.z + j * 1.5f;
                    enemyGameobject.transform.position = enemyPositionOnSpawnPoint;

                }
            }
        }
    }
}

[System.Serializable]
public class EnemySpawnPoints
{
    public Transform spawnPoint;
    public float  xAxisRight, zAxisDown;
}
