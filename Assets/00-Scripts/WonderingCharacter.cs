using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class WonderingCharacter : MonoBehaviour
{
    #region Fields

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;

    private int _idleHash = Animator.StringToHash("Idle");
    private int _walkHash = Animator.StringToHash("Walk");

    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;

    [SerializeField] private float _minWaitTime;
    [SerializeField] private float _maxWaitTime;
    [SerializeField] private float _moveSpeed;

    private Vector3 _targetPoint;


    private Transform _transform;
    private Coroutine _walkRoutine;

    #endregion

    #region Unity actions

    private void Awake()
    {
        _agent.speed = _moveSpeed;
        _transform = transform;
    }

    private void Start()
    {
        FindTarget();
    }

    #endregion

    #region Methods

    private async void FindTarget()
    {
        _targetPoint = await GetRandomDestPoint();
        CreateDestPoint(_targetPoint);
        _agent.SetDestination(_targetPoint);
        _animator.SetTrigger(_walkHash);
        if (_walkRoutine != default)
            StopCoroutine(_walkRoutine);
        _walkRoutine = StartCoroutine(CheckIfReachedDest());
    }

    async Task<Vector3> GetRandomDestPoint()
    {
        var distance = GetRandomDistance();
        var destPoint = distance * GetRandomDirection() + _transform.position;
        var path = new NavMeshPath();
        if (NavMesh.SamplePosition(destPoint, out NavMeshHit hit, distance, NavMesh.AllAreas))
        {
            if (NavMesh.CalculatePath(_transform.position, destPoint, NavMesh.AllAreas, path))
                return hit.position;
        }

        await Task.Yield();
        return await GetRandomDestPoint();
    }

    float GetRandomDistance()
    {
        return Random.Range(_minDistance, _maxDistance);
    }

    Vector3 GetRandomDirection()
    {
        return Random.onUnitSphere;
    }

    float GetRandomWaitTime()
    {
        return Random.Range(_minWaitTime, _maxWaitTime);
    }

    IEnumerator CheckIfReachedDest()
    {
        var reached = false;
        yield return null;
        while (!reached)
        {
            if (_agent.ReachedDestinationOrGaveUp())
                reached = true;
            yield return null;
        }

        _animator.SetTrigger(_idleHash);
        yield return new WaitForSeconds(GetRandomWaitTime());
        FindTarget();
    }

    void CreateDestPoint(Vector3 pos)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.position = pos;
        obj.GetComponent<Renderer>().sharedMaterial.color = Color.red;
        obj.transform.localScale = Vector3.one * .2f;
    }

    #endregion
}