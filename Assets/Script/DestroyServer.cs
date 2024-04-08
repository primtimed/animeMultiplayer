using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroyServer : MonoBehaviour
{
        public float _time;

    private void Start()
    {
        Invoke("DestroyDelay", _time);
    }

    void DestroyDelay()
    {
        Destroy(gameObject);
    }
}
