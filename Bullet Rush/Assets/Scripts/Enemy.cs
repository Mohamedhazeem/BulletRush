using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed, rotationSpeed;
    [SerializeField]private Transform player;
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
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.position),rotationSpeed*Time.deltaTime);
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Obstacles"))
        {
            Debug.LogWarning("Enter");
           // StartCoroutine(Move(true));
        }
        else
        {
           // StopCoroutine(Move(false));
            //StopAllCoroutines();
        }
        
    }

    void RemoveFromEnemiesList()
    {
        EnemyManager.instance.enemyList.Remove(this.transform);
    }

    //IEnumerator Move(bool isTrue)
    //{
    //    while (isTrue == true)
    //    {
    //        Debug.LogError("W");
    //        transform.position += transform.right * moveSpeed * Time.deltaTime;
    //        yield return new WaitForSeconds(0.01f);
    //    }

    //}
}
