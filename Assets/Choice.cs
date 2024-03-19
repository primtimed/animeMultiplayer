using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Choice : MonoBehaviour
{
    public GameObject _weapon;
    public GameObject _abbilities;
    public GameObject _team;

    private void OnEnable()
    {
        _weapon.SetActive(true);
        _abbilities.SetActive(false);
        _team.SetActive(false);
    }
}
