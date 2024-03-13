using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed = 0.05f;
    [SerializeField] private Transform cameraArm;
    [SerializeField] private Vector2 cameraRotationSpeed;
    [SerializeField] private Vector2 cameraMaxDeviation;
    [SerializeField] private Vector2 cameraReturnForce;
    [SerializeField] private GameObject barrel;
    [SerializeField] private GameObject destructionBarrel;

    private GameManager _gameManager;
    private Vector2 _movement;
    private Animator _animator;
    private Animator _barrelAnimator;
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    [NonSerialized] public bool canMove;
    private static readonly int OnFail = Animator.StringToHash("onFail");
    private static readonly int OnWin = Animator.StringToHash("onWin");
    public bool IsVisible => _movement.magnitude > 0;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _animator = GetComponent<Animator>();
        _barrelAnimator = GetComponentsInChildren<Animator>()[1];
        Input.multiTouchEnabled = true;
    }

    private void Update()
    {
        InputSystem.Update();
    }

    private void FixedUpdate()
    {
        var cameraRot = SignedEulerAngles(cameraArm.localRotation.eulerAngles);
        cameraArm.localRotation = Quaternion.Euler(Vector3.Scale(cameraRot, Vector2.one - cameraReturnForce));
        
        if (_movement.magnitude == 0) return;
        var stickRotation = Mathf.Atan2(_movement.x, _movement.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(
            transform.rotation.eulerAngles.y + stickRotation * rotationSpeed,
            Vector3.up);
        transform.Translate(speed * Time.fixedDeltaTime * Vector3.forward);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        var rot = Vector2.Scale(context.ReadValue<Vector2>().normalized, cameraRotationSpeed * Time.deltaTime);
        var current = SignedEulerAngles(cameraArm.localRotation.eulerAngles);
        var newX = current.x + rot.y;
        var newY = current.y - rot.x;
        cameraArm.localRotation = Quaternion.Euler(
            Mathf.Abs(newX) <= cameraMaxDeviation.y ? newX : Mathf.Sign(newX) * cameraMaxDeviation.y,
            Mathf.Abs(newY) <= cameraMaxDeviation.x ? newY : Mathf.Sign(newY) * cameraMaxDeviation.x,
            0
        );
    }

    private static Vector3 SignedEulerAngles(Vector3 eulerAngles)
    {
        if (eulerAngles.x > 180f) eulerAngles.x -= 360f;
        if (eulerAngles.y > 180f) eulerAngles.y -= 360f;
        if (eulerAngles.z > 180f) eulerAngles.z -= 360f;
        return eulerAngles;
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
        _gameManager.StopGame(false);
    }

    public void Win()
    {
        Stop();
        _animator.SetTrigger(OnWin);
        _barrelAnimator.SetTrigger(OnWin);
    }
}