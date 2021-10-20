using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Rigidbody playerParentRigidbody;

    [SerializeField] private float rotateSpeed;
    [SerializeField] private float speed;

    [SerializeField] private Transform rightHandParent;
    [SerializeField] private Transform leftHandParent;

    [SerializeField] private GameObject rightHandPivot;

    Dictionary<Transform, float> unsortedDictionary = new Dictionary<Transform, float>();
    public float timeToClearList;
    private float totalTimeToClearList;
    public List<Transform> nearestEnemyList;

    public Transform playerForwardPoint;
    private float totalAngle = 0f;
    private float angleBetweenBodyAndForwardPoint = 0f;
    private Vector3 offset;
    private float angle;
    RaycastHit hit;
    void Start()
    {
        playerParentRigidbody = transform.parent.GetComponent<Rigidbody>();
        InputManager.instance.OnRotatePlayer += Rotate;
        InputManager.instance.OnMovePlayer += Move;

        totalTimeToClearList = timeToClearList;
        rightHandParent = transform.Find("RightHandHolder");
        leftHandParent = transform.Find("LeftHandHolder");
       // rightHandPivot = transform.GetChild(2).Find("Pivot").gameObject;
        nearestEnemyList = new List<Transform>();
        StartCoroutine("NearestEnemy");
    }
    
    private void Update()
    {

        CreateEnemyList();
       // FindNearestEnemy();
        Shoot();

        //ClearEnemyList();
    }
    private void Rotate()
    {
        if(nearestEnemyList.Count == 0)
        {
            return;
        }
        offset = nearestEnemyList[0].position - transform.position;
        // roate toward between two enemy common points
        angle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        Debug.LogWarning(angle);
        HandRotateTowardEnemy();
        if (Vector3.Distance(transform.position, nearestEnemyList[0].position) < 8)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.up), Time.deltaTime * rotateSpeed);
            
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(InputManager.instance.angle, Vector3.up), Time.deltaTime * rotateSpeed);
            HandRotateTowardInitialRotation();
        }
    }

    private void HandRotateTowardInitialRotation()
    {
        var differenceForRightHand = playerForwardPoint.position- rightHandParent.position;
        var differenceForLeftHand = playerForwardPoint.position - leftHandParent.position;

        var angleForRightHand = Mathf.Atan2(differenceForRightHand.x, differenceForRightHand.z) * Mathf.Rad2Deg;
        var angleForLeftHand = Mathf.Atan2(differenceForLeftHand.x, differenceForLeftHand.z) * Mathf.Rad2Deg;

        rightHandParent.rotation = Quaternion.Lerp(rightHandParent.rotation, Quaternion.AngleAxis(angleForRightHand, Vector3.up), Time.deltaTime * rotateSpeed);
        leftHandParent.rotation = Quaternion.Lerp(leftHandParent.rotation, Quaternion.AngleAxis(angleForLeftHand, Vector3.up), Time.deltaTime * rotateSpeed);
    }

    private void Move()
    {
        var i = InputManager.instance.dragDirection * Time.deltaTime * speed;
        playerParentRigidbody.velocity = i;
        var differenceForRightHand = playerForwardPoint.position - transform.position;
        var angleForRightHand = Mathf.Atan2(differenceForRightHand.x, differenceForRightHand.z) * Mathf.Rad2Deg;
        Debug.Log($"differencefor righthand {angleForRightHand}");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position +transform.forward*5 , 20);
    }

    private void CreateEnemyList()
    {
        if (Physics.SphereCast(transform.position, 15, transform.forward, out hit, 10, PlayerManager.instance.layerMask))
        {

            if (EnemyManager.instance.enemyList.Contains(hit.transform))
            {
               // FindNearestEnemy();
                return;
            }
            else
            {
                
                EnemyManager.instance.enemyList.Add(hit.transform);
                //FindNearestEnemy();
            }
        } 
        
    }
    private void FindNearestEnemy()
    {
        if (EnemyManager.instance.enemyList.Count > 0)
        {
            nearestEnemyList.Clear();
            unsortedDictionary.Clear();
            for (int i = 0; i < EnemyManager.instance.enemyList.Count; i++)
            {               
                float distance = Vector3.Distance(transform.position, EnemyManager.instance.enemyList[i].position);
               
                unsortedDictionary[EnemyManager.instance.enemyList[i]] = distance;
                Debug.Log(unsortedDictionary.Count);
            }
            foreach (var item in unsortedDictionary.OrderBy(key => key.Value))
            {
                nearestEnemyList.Add(item.Key);
            }
            Debug.LogError(nearestEnemyList[0]);
        }
    } // old  nearest enemy logic
    private void Shoot()
    {
        
        // 0  to 90 right 
        // o to - 90 left

        if (angle > 0 && angle < 90)
        {
            //if(Time.time >)
            
            //var gameObject = ObjectPoolManager.instance.GetObjectFromPool(PlayerManager.instance.bullectReference);
            //gameObject.transform.position = rightHandPivot.transform.position;
            //gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * speed);

        }
        else if (angle < 0 && angle > -90)
        {

        }
    }
    private void HandRotateTowardEnemy()
    {
        
        if (nearestEnemyList.Count == 0)
        {
            return;
        }

        if (RightHandNearestEnemy() != null)
        {
            var differenceForRightHand = nearestEnemyList[(int)RightHandNearestEnemy()].position - rightHandParent.position;
            var angleForRightHand = Mathf.Atan2(differenceForRightHand.x, differenceForRightHand.z) * Mathf.Rad2Deg;
            rightHandParent.rotation = Quaternion.Lerp(rightHandParent.rotation, Quaternion.AngleAxis(angleForRightHand, Vector3.up), Time.deltaTime * rotateSpeed);
            Debug.LogError("Right");

            HandRotateTowardInitialRotation();
        }
        
        if(LeftHandNeatestEnemy() != null)
        {
            var differenceForLeftHand = nearestEnemyList[(int)LeftHandNeatestEnemy()].position - leftHandParent.position;
            var angleForLeftHand = Mathf.Atan2(differenceForLeftHand.x, differenceForLeftHand.z) * Mathf.Rad2Deg;
            leftHandParent.rotation = Quaternion.Lerp(leftHandParent.rotation, Quaternion.AngleAxis(angleForLeftHand, Vector3.up), Time.deltaTime * rotateSpeed);
            Debug.LogError("Left");
            HandRotateTowardInitialRotation();
        }
            
           
    }
    private int? RightHandNearestEnemy()
    {
        var differencebetweenBodyAndForwardPoint = playerForwardPoint.position - transform.position;
        for (int i = 0; i < nearestEnemyList.Count; i++)
        {
            var differencebetweenBodyAndNearEnemy = nearestEnemyList[i].position - transform.position;
            angleBetweenBodyAndForwardPoint = Mathf.Atan2(differencebetweenBodyAndForwardPoint.x, differencebetweenBodyAndForwardPoint.z) * Mathf.Rad2Deg;
            var angleBetweenBoydAndEnemy = Mathf.Atan2(differencebetweenBodyAndNearEnemy.x, differencebetweenBodyAndNearEnemy.z) * Mathf.Rad2Deg;

            totalAngle = angleBetweenBodyAndForwardPoint + angleBetweenBoydAndEnemy;

            if (totalAngle > 0 && totalAngle < angleBetweenBodyAndForwardPoint + 90)
            {

                return i;
            }
        }
        return null;
    }
    private int? LeftHandNeatestEnemy()
    {
        var differencebetweenBodyAndForwardPoint = playerForwardPoint.position - transform.position;
        for (int i = 0; i < nearestEnemyList.Count; i++)
        {
            var differencebetweenBodyAndNearEnemy = nearestEnemyList[i].position - transform.position;
            angleBetweenBodyAndForwardPoint = Mathf.Atan2(differencebetweenBodyAndForwardPoint.x, differencebetweenBodyAndForwardPoint.z) * Mathf.Rad2Deg;
            var angleBetweenBoydAndEnemy = Mathf.Atan2(differencebetweenBodyAndNearEnemy.x, differencebetweenBodyAndNearEnemy.z) * Mathf.Rad2Deg;

            totalAngle = angleBetweenBodyAndForwardPoint + angleBetweenBoydAndEnemy;

            if (totalAngle < 0 && totalAngle > angleBetweenBodyAndForwardPoint - 90)
            {
                return i;
            }
        }
        return null;
    }
    IEnumerator NearestEnemy()
    {
        while (true)
        {
            if (EnemyManager.instance.enemyList.Count > 0)
            {
                nearestEnemyList.Clear();
                unsortedDictionary.Clear();
                for (int i = 0; i < EnemyManager.instance.enemyList.Count; i++)
                {
                    float distance = Vector3.Distance(transform.position, EnemyManager.instance.enemyList[i].position);

                    unsortedDictionary[EnemyManager.instance.enemyList[i]] = distance;
                }
                foreach (var item in unsortedDictionary.OrderBy(key => key.Value))
                {
                    nearestEnemyList.Add(item.Key);
                }
                
            }
            yield return new WaitForSeconds(0.5f);
        }
        
    }
}
