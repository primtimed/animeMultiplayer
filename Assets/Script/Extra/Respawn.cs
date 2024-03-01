using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [HideInInspector] public Transform[] _spawns;

    private void Start()
    {
        _spawns = GetComponentsInChildren<Transform>();
    }
}
