using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyObject : MonoBehaviour
{
    public float timeToDestroy = 5f;

    void Start()
    {
        StartCoroutine(DestroyObj());
    }

    private IEnumerator DestroyObj()
    {
        yield return new WaitForSecondsRealtime(timeToDestroy);

        Destroy(gameObject);
    }


}
