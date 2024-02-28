using System.Collections;
using UnityEngine;

public class destroy : MonoBehaviour
{
    public float _time;

    private void Start()
    {
        StartCoroutine(DestroyDelay());
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(_time);
        Destroy(gameObject);
    }
}
