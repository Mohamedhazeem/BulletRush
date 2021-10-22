using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Rigidbody playerParentRigidbody;

    [SerializeField] private float rotateSpeed;
    [SerializeField] private float dragSpeed;
    [SerializeField] private float speed;

    [SerializeField] private Transform rightHandParent;
    [SerializeField] private Transform leftHandParent;

    [SerializeField] private GameObject rightHandPivot;
    [SerializeField] private GameObject leftHandPivot;

    Dictionary<Transform, float> unsortedDictionary = new Dictionary<Transform, float>();

    public List<Transform> nearestEnemyList;

    public Transform playerForwardPoint;
    private float totalAngleForRightHand = 0f;
    private float totalAngleForBodyAndEnemy;
    private float totalAngleForLeftHand = 0f;
    private float angleBetweenBodyAndForwardPoint = 0f;
    private Vector3 offset;
    private float angle;
    RaycastHit hit;
    [SerializeField] private float shootInterval;
    private float nextTime;
    private float nextTime1;

    void Start()
    {
        playerParentRigidbody = transform.parent.GetComponent<Rigidbody>();
       // InputManager.instance.OnRotatePlayer += Rotate;
        InputManager.instance.OnMovePlayer += Move;

        nextTime = shootInterval;
        nextTime1 = shootInterval;
        //rightHandParent = transform.Find("RightHandHolder");
        //leftHandParent = transform.Find("LeftHandHolder");

        nearestEnemyList = new List<Transform>();
        StartCoroutine("NearestEnemy");
    }
    
    private void Update()
    {
        CreateEnemyList();
        Rotate();
    }
    private void Move()
    {
        var i = InputManager.instance.dragDirection * Time.deltaTime * dragSpeed;
        playerParentRigidbody.velocity = i;
    }
    private void Rotate()
    {
        if(nearestEnemyList.Count == 0)
        {
            return;
        }

        if(nearestEnemyList.Count <= 1)
        {
            offset = nearestEnemyList[0].position - transform.position;
            // roate toward between two enemy common points
            angle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;

            if (Vector3.Distance(transform.position, nearestEnemyList[0].position) < 10)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.up), Time.deltaTime * rotateSpeed);
                RightHandRotateTowardEnemy();
                LeftHandRotateTowardEnemy();
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(InputManager.instance.angle, Vector3.up), Time.deltaTime * rotateSpeed);
                HandRotateTowardInitialRotation();
            }
        }
        else if (nearestEnemyList.Count > 1)
        {
            if (Vector3.Distance(transform.position, nearestEnemyList[0].position)> 5 && Vector3.Distance(transform.position, nearestEnemyList[1].position) > 5)
            {

                var commonPoint = nearestEnemyList[0].position + nearestEnemyList[1].position;
                commonPoint /= 2;

                offset = commonPoint - transform.position;

                angle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.up), Time.deltaTime * rotateSpeed);
                    RightHandRotateTowardEnemy();
                    LeftHandRotateTowardEnemy();

            }
            else
            {
                offset = nearestEnemyList[0].position - transform.position;

                angle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;

                if (Vector3.Distance(transform.position, nearestEnemyList[0].position) < 10)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.up), Time.deltaTime * rotateSpeed);
                    RightHandRotateTowardEnemy();
                    LeftHandRotateTowardEnemy();
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(InputManager.instance.angle, Vector3.up), Time.deltaTime * rotateSpeed);
                    HandRotateTowardInitialRotation();
                }
            }

        }

    }
    private void RightHandRotateTowardEnemy()
    {
        if (nearestEnemyList.Count == 0)
        {
            return;
        }

        int? rightEnemy = RightHandNearestEnemy();
        int? leftEnemy = LeftHandNeatestEnemy();
        if (rightEnemy != null)
        {
            var differenceForRightHand = nearestEnemyList[(int)rightEnemy].position - rightHandParent.position;
            var angleForRightHand = Mathf.Atan2(differenceForRightHand.x, differenceForRightHand.z) * Mathf.Rad2Deg;
            rightHandParent.rotation = Quaternion.Lerp(rightHandParent.rotation, Quaternion.AngleAxis(angleForRightHand, Vector3.up), Time.deltaTime * rotateSpeed);
            ShootRightHand();
        }
        else
        {
            HandRotateTowardInitialRotation();
            if(LeftHandNeatestEnemy() != null && Vector3.Distance(nearestEnemyList[(int)leftEnemy].position, leftHandParent.position) < 10)
            {
                ShootRightHand();
            }            
        }
    }
    private void LeftHandRotateTowardEnemy()
    {
        if (nearestEnemyList.Count == 0)
        {
            return;
        }

        int? leftEnemy = LeftHandNeatestEnemy();
        int? rightEnemy = RightHandNearestEnemy();
        if (leftEnemy != null)
        {
            var differenceForLeftHand = nearestEnemyList[(int)leftEnemy].position - leftHandParent.position;
            var angleForLeftHand = Mathf.Atan2(differenceForLeftHand.x, differenceForLeftHand.z) * Mathf.Rad2Deg;

            leftHandParent.rotation = Quaternion.Lerp(leftHandParent.rotation, Quaternion.AngleAxis(angleForLeftHand, Vector3.up), Time.deltaTime * rotateSpeed);
           ShootLeftHand();
        }
        else
        {
            HandRotateTowardInitialRotation();
            if(RightHandNearestEnemy() != null && Vector3.Distance(nearestEnemyList[(int)rightEnemy].position, leftHandParent.position) < 10)
            {
                ShootLeftHand();
            }
            
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


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position +transform.forward*5 , 20);
    }

    private void CreateEnemyList()
    {
        if (Physics.SphereCast(transform.parent.position, 15, transform.forward, out hit, 10, PlayerManager.instance.layerMask))
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
        else if(Physics.SphereCast(transform.parent.position, 15, -transform.forward, out hit, 10, PlayerManager.instance.layerMask))
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
    private void ShootRightHand()
    {       
         if (Time.time > nextTime) 
         {
            nextTime = Time.time + shootInterval;

            var gameObject = ObjectPoolManager.instance.GetObjectFromPool(PlayerManager.instance.bullectReference);
            gameObject.transform.position = rightHandPivot.transform.position;
            gameObject.GetComponent<Bullet>().bulletForward = rightHandPivot.transform.forward;

         }
    }
    private void ShootLeftHand()
    {
        if (Time.time > nextTime1)
        {
            nextTime1 = Time.time + shootInterval;

            var gameObject = ObjectPoolManager.instance.GetObjectFromPool(PlayerManager.instance.bullectReference);
            gameObject.transform.position = leftHandPivot.transform.position;
            gameObject.GetComponent<Bullet>().bulletForward = leftHandPivot.transform.forward;         
        }
    }

    private int? RightHandNearestEnemy()
    {
        
        for (int i = 0; i < nearestEnemyList.Count; i++)
        {
            var differencebetweenBodyAndForwardPoint = playerForwardPoint.position - transform.position;
            var differencebetweenBodyAndNearEnemy = nearestEnemyList[i].position - transform.position;

            var angleBetweenBodyAndForwardPoint = Mathf.Atan2(differencebetweenBodyAndForwardPoint.x, differencebetweenBodyAndForwardPoint.z) * Mathf.Rad2Deg;
            var angleBetweenBodyAndEnemy = Mathf.Atan2(differencebetweenBodyAndNearEnemy.x, differencebetweenBodyAndNearEnemy.z) * Mathf.Rad2Deg;

            angleBetweenBodyAndForwardPoint= Clamp0To360(angleBetweenBodyAndForwardPoint);
            angleBetweenBodyAndEnemy = Clamp0To360(angleBetweenBodyAndEnemy);

            Debug.LogError($"angleBetweenBodyAndForwardPoint = {angleBetweenBodyAndForwardPoint}");

            totalAngleForRightHand = angleBetweenBodyAndForwardPoint + 90;
         
            if(totalAngleForRightHand > 360)
            {
                totalAngleForRightHand -= 360;
            }
            Debug.LogError($"angleBetweenBodyAndEnemy = {angleBetweenBodyAndEnemy}");
            Debug.LogError($"right hand total angle = {totalAngleForRightHand}");
            if (angleBetweenBodyAndEnemy < totalAngleForRightHand )
            {
                Debug.LogError($"right hand nearest enemy = {nearestEnemyList[i]}");
                return i;
            }
        }
        return null;
    }

    private int? LeftHandNeatestEnemy()
    {
        for (int i = 0; i < nearestEnemyList.Count; i++)
        {
            var differencebetweenBodyAndForwardPoint = playerForwardPoint.position - transform.position;
            var differencebetweenBodyAndNearEnemy = nearestEnemyList[i].position - transform.position;

            var angleBetweenBodyAndForwardPoint = Mathf.Atan2(differencebetweenBodyAndForwardPoint.x, differencebetweenBodyAndForwardPoint.z) * Mathf.Rad2Deg;
            var angleBetweenBodyAndEnemy = Mathf.Atan2(differencebetweenBodyAndNearEnemy.x, differencebetweenBodyAndNearEnemy.z) * Mathf.Rad2Deg;

            angleBetweenBodyAndForwardPoint = Clamp0To360(angleBetweenBodyAndForwardPoint);
            angleBetweenBodyAndEnemy = Clamp0To360(angleBetweenBodyAndEnemy);

            Debug.LogError($"angleBetweenBodyAndForwardPoint = {angleBetweenBodyAndForwardPoint}");
            totalAngleForLeftHand = angleBetweenBodyAndForwardPoint - 90;

            totalAngleForLeftHand = 360 - totalAngleForLeftHand;

            Debug.LogError($"angleBetweenBodyAndEnemy = {angleBetweenBodyAndEnemy}");
            Debug.LogError($"left hand total angle = {totalAngleForLeftHand}");

            if (angleBetweenBodyAndEnemy < 360 && angleBetweenBodyAndEnemy > totalAngleForLeftHand)
            {
                Debug.LogError($"left hand nearest enemy = {nearestEnemyList[i]}");
                return i;
            }
        }
        return null;
    }
    public static float Clamp0To360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
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
            yield return new WaitForSeconds(0.1f);
        }
        
    }
}
