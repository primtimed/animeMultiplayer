using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    Rigidbody _rb;
    public TextMeshProUGUI _speedUI, _abbilUI;

    public float _time;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        StartCoroutine(Speed());
    }

    public IEnumerator Speed()
    {
        _speedUI.text = "Speed  " + (int)_rb.velocity.magnitude;
        yield return new WaitForSeconds(.2f);

        StartCoroutine(Speed());
    }

    private void Update()
    {
        if (_time <= 0)
        {
            _abbilUI.text = _time.ToString("f1");
        }

        else
        {
            _abbilUI.text = null;
        }
    }
}
