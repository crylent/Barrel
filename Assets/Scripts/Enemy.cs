using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    [SerializeField] private List<Vector2> waypoints;
    [SerializeField] private float restTime = 2f;
    [SerializeField] private float speed = 3f;

    private Animator _animator;

    private bool _isPatrolling;
    private bool _isTakingRest;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int OnShootTrigger = Animator.StringToHash("onShoot");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(Patrol());
    }

    private void FixedUpdate()
    {
        if (!_isTakingRest) transform.Translate(speed * Time.fixedDeltaTime * Vector3.forward);
    }

    private IEnumerator Patrol()
    {
        _isPatrolling = true;
        var i = 0;
        while (_isPatrolling)
        {
            Walk(waypoints[i], out var time);
            yield return new WaitForSeconds(time);
            StopWalking();
            yield return new WaitForSeconds(restTime);
            i = (i + 1) % waypoints.Count;
        }
    }

    private void Walk(Vector2 waypoint, out float time)
    {
        var position = transform.position;
        var posDiff = new Vector3(waypoint.x - position.x, 0, waypoint.y - position.z);
        transform.rotation = Quaternion.AngleAxis(
            Mathf.Atan2(posDiff.x, posDiff.z) * Mathf.Rad2Deg,
            Vector3.up);
        _isTakingRest = false;
        _animator.SetBool(IsWalking, true);
        time = posDiff.magnitude / speed;
    }

    private void StopWalking()
    {
        _isTakingRest = true;
        _animator.SetBool(IsWalking, false);
    }

    private bool _shootIsDone;

    private IEnumerator Shoot(PlayerController player)
    {
        var posDiff = player.transform.position - transform.position;
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(posDiff.x, posDiff.z) * Mathf.Rad2Deg, Vector3.up);
        _animator.SetTrigger(OnShootTrigger);
        yield return new WaitUntil(() => _shootIsDone);
        player.Fail();
    }

    public void OnShoot()
    {
        _shootIsDone = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var player = other.GetComponent<PlayerController>();
        if (!player.IsVisible) return;

        _isPatrolling = false;
        StopWalking();
        StartCoroutine(Shoot(player));
        player.Stop();
    }
}