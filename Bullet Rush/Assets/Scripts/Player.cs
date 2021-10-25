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

    public Transform playerForwardPoint;
    [SerializeField] private float shootInterval;

    Dictionary<Transform, float> unsortedDictionary = new Dictionary<Transform, float>();

    public List<Transform> nearestEnemyList;
    
    private float totalAngleForRightHand = 0f;
    private float totalAngleForLeftHand = 0f;
    private float angleBetweenBodyAndForwardPoint = 0f;

    private Vector3 offset;
    private float angle;
    RaycastHit hit;

    private float nextTimeForRightShoot;
    private float nextTimeForLeftShoot;

    void Start()
    {

        playerParentRigidbody = transform.parent.GetComponent<Rigidbody>();
        nearestEnemyList = new List<Transform>();

        InputManager.instance.OnMovePlayer += Move;

        nextTimeForRightShoot = shootInterval;
        nextTimeForLeftShoot = shootInterval;

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
        if (nearestEnemyList.Count == 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(InputManager.instance.angle, Vector3.up), Time.deltaTime * rotateSpeed);
        }
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
            if (Vector3.Distance(transform.position, nearestEnemyList[0].position) < 20 && Vector3.Distance(transform.position, nearestEnemyList[1].position) < 20 )
            {

                var commonPoint = nearestEnemyList[0].position + nearestEnemyList[1].position;
                commonPoint /= 2;

                offset = commonPoint - transform.position;

                angle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.up), Time.deltaTime * rotateSpeed);
                    RightHandRotateTowardEnemy();
                    LeftHandRotateTowardEnemy();

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
        }
    }
    private void LeftHandRotateTowardEnemy()
    {
        if (nearestEnemyList.Count == 0)
        {
            return;
        }

        int? leftEnemy = LeftHandNeatestEnemy();

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
        }
    }
    private void HandRotateTowardInitialRotation()
    {
        if (LeftHandNeatestEnemy() == null)
        {
            ShootLeftHand();
        }
        else if (RightHandNearestEnemy() == null)
        {
            ShootRightHand();
        }     
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
                return;
            }
            else
            {
                
                EnemyManager.instance.enemyList.Add(hit.transform);
            }
        }
        else if(Physics.SphereCast(transform.parent.position, 15, -transform.forward, out hit, 10, PlayerManager.instance.layerMask))
        {
            if (EnemyManager.instance.enemyList.Contains(hit.transform))
            {               
                return;
            }
            else
            {
                EnemyManager.instance.enemyList.Add(hit.transform);
            }
        }
   
        
    }
    private void ShootRightHand()
    {       
         if (Time.time > nextTimeForRightShoot) 
         {
            nextTimeForRightShoot = Time.time + shootInterval;
            var gameObject = ObjectPoolManager.instance.GetObjectFromPool(PlayerManager.instance.bulletReference);
            gameObject.transform.position = rightHandPivot.transform.position;
            gameObject.GetComponent<Bullet>().bulletForward = rightHandPivot.transform.forward;

         }
    }
    private void ShootLeftHand()
    {
        if (Time.time > nextTimeForLeftShoot)
        {
            nextTimeForLeftShoot = Time.time + shootInterval;
            var gameObject = ObjectPoolManager.instance.GetObjectFromPool(PlayerManager.instance.bulletReference);
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

            angleBetweenBodyAndForwardPoint = Mathf.Atan2(differencebetweenBodyAndForwardPoint.x, differencebetweenBodyAndForwardPoint.z) * Mathf.Rad2Deg;
            var angleBetweenBodyAndEnemy = Mathf.Atan2(differencebetweenBodyAndNearEnemy.x, differencebetweenBodyAndNearEnemy.z) * Mathf.Rad2Deg;

            totalAngleForRightHand = angleBetweenBodyAndForwardPoint + 90;
            totalAngleForRightHand = Clamp0To360(totalAngleForRightHand);
            angleBetweenBodyAndForwardPoint = Clamp0To360(angleBetweenBodyAndForwardPoint);
            angleBetweenBodyAndEnemy = Clamp0To360(angleBetweenBodyAndEnemy);
                    
            if(totalAngleForRightHand > 360)
            {
                totalAngleForRightHand -= 360;
            }

            if ( angleBetweenBodyAndEnemy < totalAngleForRightHand)
            {
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
            var differencebetweenBodyAndNearEnemy =  nearestEnemyList[i].position - transform.position;

            var angleBetweenBodyAndForwardPoint = Mathf.Atan2(differencebetweenBodyAndForwardPoint.x, differencebetweenBodyAndForwardPoint.z) * Mathf.Rad2Deg;
            var angleBetweenBodyAndEnemy = Mathf.Atan2(differencebetweenBodyAndNearEnemy.x, differencebetweenBodyAndNearEnemy.z) * Mathf.Rad2Deg;

            totalAngleForLeftHand = angleBetweenBodyAndForwardPoint - 90;
            totalAngleForLeftHand = Clamp0To360(totalAngleForLeftHand);
            angleBetweenBodyAndForwardPoint = Clamp0To360(angleBetweenBodyAndForwardPoint);
            angleBetweenBodyAndEnemy = Clamp0To360(angleBetweenBodyAndEnemy);

            if ( angleBetweenBodyAndEnemy > totalAngleForLeftHand && angleBetweenBodyAndEnemy < 360)
            {             
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
            else
            {
                nearestEnemyList.Clear();
                unsortedDictionary.Clear();
            }
            yield return new WaitForSeconds(0.1f);
        }
        
    }
}
