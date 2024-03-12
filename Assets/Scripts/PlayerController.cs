using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed = 0.05f;
    private Vector2 _movement;
    private Camera _camera;

    public bool IsVisible => _movement.magnitude > 0;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        InputSystem.Update();
    }

    private void FixedUpdate()
    {
        if (_movement.magnitude == 0) return;
        
        var stickRotation = Mathf.Atan2(_movement.x, _movement.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(
            transform.rotation.eulerAngles.y + stickRotation * rotationSpeed,
            Vector3.up);
        transform.Translate(speed * Time.fixedDeltaTime * Vector3.forward, _camera.transform);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }
}