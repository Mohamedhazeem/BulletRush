using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed, rotationSpeed;
    [SerializeField]private GameObject player;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        player = PlayerManager.instance.currentPlayer;
    }
    void Update()
    {
        // var distance = Vector3.Distance(transform.position, player.position);

        this.transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);

        var rotation = Quaternion.LookRotation(player.transform.position - transform.position);

       // rotation = Quaternion.Euler(0,rotation.y,rotation.z);

        transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            RemoveFromEnemiesList();
            ObjectPoolManager.instance.ReturnToObjectPool(this.gameObject);
        }    
    }

    void RemoveFromEnemiesList()
    {
        EnemyManager.instance.enemyList.Remove(this.transform);
    }

}
