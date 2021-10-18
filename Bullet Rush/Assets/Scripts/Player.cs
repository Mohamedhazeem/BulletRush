using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody playerParentRigidbody;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float speed;
    [SerializeField] private Transform rightHandParent;
    [SerializeField] private Transform leftHandParent;

    public float timeToClearList;
    private float totalTimeToClearList;
    public List<float> nearestEnemyList;

    private Vector3 offset;
    private float angle;
    RaycastHit hit;
    void Start()
    {
        playerParentRigidbody = transform.parent.GetComponent<Rigidbody>();
        InputManager.instance.OnRotatePlayer += Rotate;
        InputManager.instance.OnMovePlayer += Move;

        totalTimeToClearList = timeToClearList;

        nearestEnemyList = new List<float>();
       // StartCoroutine(NearEnemy());
    }
    
    private void Update()
    {
        CreateEnemyList();
        Shoot();
        //ClearEnemyList();
    }
    private void Rotate()
    {
        offset = EnemyManager.instance.enemyList[0].position - transform.position;
        angle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;

        if (Vector3.Distance(transform.position, EnemyManager.instance.enemyList[0].position) < 8)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.up), Time.deltaTime * rotateSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(InputManager.instance.angle, Vector3.up), Time.deltaTime * rotateSpeed);
        }
    }
    private void Move()
    {
        var i = InputManager.instance.dragDirection * Time.deltaTime * speed;
        playerParentRigidbody.velocity = i;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position +transform.forward*5 , 20);
    }

    private void CreateEnemyList()
    {
        if (Physics.SphereCast(transform.position, 20, transform.forward, out hit, 20, PlayerManager.instance.layerMask))
        {
            if (EnemyManager.instance.enemyList.Contains(hit.transform))
            {
               // FindNearestEnemy();
                return;
            }
            else
            {
                EnemyManager.instance.enemyList.Add(hit.transform);
                FindNearestEnemy();
            }
        } 
        
    }
    private void FindNearestEnemy()
    {
        if (EnemyManager.instance.enemyList.Count > 0)
        {
            nearestEnemyList.Clear();
            for (int i = 0; i < EnemyManager.instance.enemyList.Count; i++)
            {               
                float distance = Vector3.Distance(transform.position, EnemyManager.instance.enemyList[i].position);
                nearestEnemyList.Add(distance);
            }
        }
    }
    private void Shoot()
    {
        var difference = EnemyManager.instance.enemyList[0].position - transform.position;

        angle = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
        Debug.Log(angle);

        if(angle > 1 && angle > -90)
        {
            //right hand
            var v =Vector3.RotateTowards(rightHandParent.position, EnemyManager.instance.enemyList[0].position, 90, 90);
            rightHandParent.rotation = Quaternion.LookRotation(v);
            //rightHandParent.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
        else if(angle > 1 && angle < 90)
        {
            leftHandParent.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            // left hand
        }
    }
}
