using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject _camera;

    public Transform _lookat;

    public Transform[] _cameraPoints;

    int _i;

    private void Start()
    {
        NewPoint();
    }

    void NewPoint()
    {
        _i = Random.Range(0, _cameraPoints.Length);

        _camera.transform.position = _cameraPoints[_i].position;

        _i = Random.Range(0, _cameraPoints.Length);
    }

    private void Update()
    {
        _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, _cameraPoints[_i].position, 10f * Time.deltaTime);
        _camera.transform.LookAt(_lookat);

        if(Vector3.Distance(_camera.transform.position, _cameraPoints[_i].position) < .1)
        {
            NewPoint();
        }
    }
}
