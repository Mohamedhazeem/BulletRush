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
       // InstantiateEnemies();
    }
    void Update()
    {
        
    }

    private void InstantiateEnemies()
    {
        foreach (EnemySpawnPoints enemySpawnpoint in enemySpawnPoints)
        {

            for (float x = -enemySpawnpoint.xAxisValue; x <= enemySpawnpoint.xAxisValue; x += 1f)
            {
                for (float z = -enemySpawnpoint.zAxisValue; z <= enemySpawnpoint.zAxisValue; z += 1f)
                {
                    for (int i = 0; i < enemySpawnpoint.enemyCount; i++)
                    {
                        enemyGameobject = ObjectPoolManager.instance.GetObjectFromPool(enemyPrefab[0]);


                    }
                    enemyPositionOnSpawnPoint.x = x;
                    enemyPositionOnSpawnPoint.y = enemySpawnPositionOnY;
                    enemyPositionOnSpawnPoint.z = z;
                    Debug.LogWarning(enemyPositionOnSpawnPoint);
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
    public int enemyCount;
    public float xAxisValue,zAxisValue;
    
    
}
