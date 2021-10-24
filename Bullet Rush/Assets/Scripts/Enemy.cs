using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed, rotationSpeed;
    [SerializeField]private GameObject player;
    void Start()
    {
        player = PlayerManager.instance.currentPlayer;
    }
    void Update()
    {
        MoveTowardPlayer();
        RotateTowardPlayer();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            RemoveFromEnemiesList();
            ObjectPoolManager.instance.ReturnToObjectPool(this.gameObject);
        }    
    }
    private void MoveTowardPlayer()
    {
        this.transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
    }
    private void RotateTowardPlayer()
    {
        var rotation = Quaternion.LookRotation(player.transform.position - transform.position);

        transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
    void RemoveFromEnemiesList()
    {
        EnemyManager.instance.enemyList.Remove(this.transform);
    }

}
