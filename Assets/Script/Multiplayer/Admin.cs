using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Admin : MonoBehaviour
{
    public bool _admin;

    [SerializeField] GameObject _console;

    private void Start()
    {
        if (_admin)
        {
            Instantiate(_console);
        }
    }
}
