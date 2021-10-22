using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine("ReturnToobjectPool");
    }
    private void OnTriggerEnter(Collider other)
    {
        transform.rotation = Quaternion.identity;
        ObjectPoolManager.instance.ReturnToObjectPool(this.gameObject);
    }

    IEnumerator ReturnToobjectPool()
    {
        yield return new WaitForSeconds(3f);
        transform.rotation = Quaternion.identity;
        ObjectPoolManager.instance.ReturnToObjectPool(this.gameObject);
    }
}

