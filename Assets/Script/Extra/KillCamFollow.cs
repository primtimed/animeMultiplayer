using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCamFollow : MonoBehaviour
{
    public Transform _player;

    private void FixedUpdate()
    {
        transform.position = new Vector3(_player.position.x, 50, _player.position.z);
        //transform.rotation = new Quaternion(-_player.localRotation.x + 90, -_player.localRotation.y, -_player.localRotation.z, -_player.localRotation.w);
    }
}
