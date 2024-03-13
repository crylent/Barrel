using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed = 0.05f;
    [SerializeField] private GameObject barrel;
    [SerializeField] private GameObject destructionBarrel;
    
    private Vector2 _movement;
    private Camera _camera;
    private Animator _animator;
    private Animator _barrelAnimator;
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    [NonSerialized] public bool canMove;
    private static readonly int OnFail = Animator.StringToHash("onFail");
    public bool IsVisible => _movement.magnitude > 0;

    private void Start()
    {
        _camera = Camera.main;
        _animator = GetComponent<Animator>();
        _barrelAnimator = GetComponentsInChildren<Animator>()[1];
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
        if (!canMove) return;
        _movement = context.ReadValue<Vector2>();
        
        if (context.started) AnimateMovement(true);
        else if (context.canceled) AnimateMovement(false);
    }

    private void AnimateMovement(bool isRunning)
    {
        _animator.SetBool(IsRunning, isRunning);
        _barrelAnimator.SetBool(IsRunning, isRunning);
    }

    public void Stop()
    {
        canMove = false;
        _movement = Vector2.zero;
        AnimateMovement(false);
    }

    public void Fail()
    {
        barrel.SetActive(false);
        destructionBarrel.SetActive(true);
        _animator.SetTrigger(OnFail);
    }
}