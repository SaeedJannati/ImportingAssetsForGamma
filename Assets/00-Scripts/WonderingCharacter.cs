using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class WonderingCharacter : MonoBehaviour
{
    #region Fields

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;

    private readonly int _idleHash = Animator.StringToHash("Idle");
    private readonly int _walkHash = Animator.StringToHash("Walk");
    private readonly int _gatherHash = Animator.StringToHash("Gathering");
    private readonly int _miningHash = Animator.StringToHash("Mining");

    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;

    [SerializeField] private float _minWaitTime;
    [SerializeField] private float _maxWaitTime;
    [SerializeField] private float _moveSpeed;

    [SerializeField] private GameObject _pickAxe;

    [SerializeField] private float _gatherChance = .1f;
    [SerializeField] private float _mineChance = .1f;

    private Vector3 _targetPoint;


    private Transform _transform;
    private Coroutine _walkRoutine;

    #endregion

    #region Unity actions

    private void Awake()
    {
        _agent.speed = _moveSpeed;
        _transform = transform;
        _pickAxe.SetActive(false);
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

        OnReachedDest();
    }

    async Task Idle(float period = -1.0f)
    {
        _animator.SetTrigger(_idleHash);
        if (period < 0)
            period = GetRandomWaitTime();
        await Task.Delay((int)(period * 1000));
    }

    async void OnReachedDest()
    {
        var nextState = GetNextState();
        await DoAction(nextState);
        FindTarget();
    }

    void CreateSphere(Vector3 pos)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.position = pos;
        obj.GetComponent<Renderer>().sharedMaterial.color = Color.red;
        obj.transform.localScale = Vector3.one * .2f;
    }

    [Button]
    async Task Gather()
    {
        _animator.SetTrigger(_gatherHash);
        await Task.Delay(2000);
    }

    [Button]
    async Task Mine()
    {
        var times = 3;
        _pickAxe.SetActive(true);
        _animator.SetTrigger(_miningHash);
        await Task.Delay(times * 1000);
        _pickAxe.SetActive(false);
        _animator.SetTrigger(_idleHash);
        await Task.Delay(1000);
        await Gather();
    }

    public enum CharacterState
    {
        Idle,
        Walk,
        Gather,
        Mine,
    }

    async Task DoAction(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Gather:
                await Gather();
                break;
            case CharacterState.Mine:
                await Mine();
                break;
            case CharacterState.Idle:
                await Idle();
                break;
        }
    }

    private CharacterState GetNextState()
    {
        if (GoGather())
            return CharacterState.Gather;
        if (GoMine())
            return CharacterState.Mine;
        return CharacterState.Idle;
    }

    private bool GoGather()
    {
        var randomNum = Random.Range(0.0f, 1.0f);
        return !(randomNum > _gatherChance);
    }

    private bool GoMine()
    {
        var randomNum = Random.Range(0.0f, 1.0f);
        return !(randomNum > _mineChance);
    }

    #endregion
}