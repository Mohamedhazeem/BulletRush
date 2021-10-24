using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private float speed;
    public Vector3 bulletForward;
 
    private void OnEnable()
    {
        StartCoroutine(ReturnToobjectPool());
    }
    private void OnDisable()
    {
        StopCoroutine(ReturnToobjectPool());
    }
    private void Update()
    {
        if(bulletForward != null)
        gameObject.transform.Translate(bulletForward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        transform.rotation = Quaternion.identity;
       // StopCoroutine("ReturnToobjectPool");
        ObjectPoolManager.instance.ReturnToObjectPool(this.gameObject);
    }

    IEnumerator ReturnToobjectPool()
    {
        yield return new WaitForSeconds(time);
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        ObjectPoolManager.instance.ReturnToObjectPool(this.gameObject);
    }
}

