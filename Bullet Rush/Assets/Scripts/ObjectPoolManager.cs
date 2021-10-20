using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ObjectPoolManager : MonoBehaviour
{

    public static ObjectPoolManager instance;

    public List<ObjectForPool> objectForPools;

    internal Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(this);
        }
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        ObjectPoolInstantiate();

    }
    private void Start()
    {
       
    }
    private void ObjectPoolInstantiate()
    {
        foreach (ObjectForPool objectPool in objectForPools)
        {

            Queue<GameObject> poolQueue = new Queue<GameObject>();
            for (int i = 0; i < objectPool.poolSize; i++)
            {
                GameObject gameObject = Instantiate(objectPool.Prefab);
                gameObject.transform.parent = this.transform;
                gameObject.SetActive(false);
                poolQueue.Enqueue(gameObject);

            }
            poolDictionary.Add(objectPool.Prefab.name, poolQueue);

        }
    }
    public GameObject GetObjectFromPool(GameObject gameObject)
    {
        if (!poolDictionary.ContainsKey(gameObject.name))
        {
            GameObject newGameObject = Instantiate(gameObject);
            newGameObject.name = gameObject.name;
            return newGameObject;
        }
        else
        {
            GameObject objectFromPool = poolDictionary[gameObject.name].Dequeue();

            objectFromPool.SetActive(true);
            poolDictionary[gameObject.name].Enqueue(objectFromPool);

            return objectFromPool;
        }
        
    }
    public void ReturnToObjectPool(GameObject gameObject)
    {
        if (!poolDictionary.ContainsKey(gameObject.name))
        {
            Queue<GameObject> poolQueue = new Queue<GameObject>();
            poolQueue.Enqueue(gameObject);
            gameObject.SetActive(false);

            poolDictionary.Add(gameObject.name, poolQueue);
        }
        else
        {
            gameObject.SetActive(false);
            poolDictionary[gameObject.name].Enqueue(gameObject);
        }
    }
}

