using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject _toFollow;

    void Update()
    {
        transform.position = _toFollow.transform.position;
    }
}
