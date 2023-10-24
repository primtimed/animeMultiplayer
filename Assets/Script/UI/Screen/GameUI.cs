using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public Rigidbody _rb;
    public TextMeshProUGUI _speedUI;

    public void StartAll()
    {
        StartCoroutine(Speed());
    }

    public IEnumerator Speed()
    {
        _speedUI.text = "Speed  " + (int)_rb.velocity.magnitude;
        yield return new WaitForSeconds(.2f);

        StartCoroutine(Speed());
    }
}
