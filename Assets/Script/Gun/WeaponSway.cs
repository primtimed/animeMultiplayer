using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    public float _smooth, _sway;

    private PlayerControlls _input;
    private Movement _move;

    private InputAction _mouse;

    private void Awake()
    {
        _input = new PlayerControlls();
        _move = GetComponentInParent<Movement>();
    }

    private void OnEnable()
    {
        _mouse = _input.Movement.Rotation;
        _mouse.Enable();
    }

    private void OnDisable()
    {
        _mouse.Disable();
    }

    private void Update()
    {
        float x = _mouse.ReadValue<Vector2>().x * (_sway * _move._sensetivitie);
        float y = _mouse.ReadValue<Vector2>().y * (_sway * _move._sensetivitie);

        Quaternion rotationX = Quaternion.AngleAxis(-y, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(x, Vector3.up);

        Quaternion target = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, _smooth * Time.deltaTime);
    }
}
