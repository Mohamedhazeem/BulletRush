using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public Rigidbody bulletRigidbody;
    [SerializeField] private float time;
    [SerializeField] private float speed;
    public Vector3 bulletForward;
    private void Start()
    {
        //time = 3f;

    }
    private void Update()
    {
        if(bulletForward != null)
        gameObject.transform.Translate(bulletForward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        transform.rotation = Quaternion.identity;
        //time = 0;
        StopCoroutine("ReturnToobjectPool");
        ObjectPoolManager.instance.ReturnToObjectPool(this.gameObject);
    }
    IEnumerator Move(GameObject pivot)
    {
        gameObject.transform.Translate(pivot.transform.forward * speed * Time.deltaTime);
        yield return new WaitForSeconds(time);
        transform.rotation = Quaternion.identity;
        ObjectPoolManager.instance.ReturnToObjectPool(this.gameObject);
    }
    //IEnumerator ReturnToobjectPool()
    //{
    //    yield return new WaitForSeconds(time);
    //    transform.rotation = Quaternion.identity;
    //    ObjectPoolManager.instance.ReturnToObjectPool(this.gameObject);
    //}
}

