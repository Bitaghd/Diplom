using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] protected Transform _player = null;
    //protected SimpleEnemyMovement Movement;
    protected NavMeshAgent Agent;
    protected Animator animator;
    [SerializeField] protected PlayerSensor followPlayerSensor;
    [SerializeField] protected PlayerSensor attackPlayerSensor;
    [SerializeField] protected EnemyScriptableObject EnemyScriptableObject;
    [SerializeField] public int Health = 100;
    protected int Damage = 5;

    private EnemyState currentState;
    private EnemyState defaultState = EnemyState.Idle;
    public EnemyState State
    {
        get
        {
            return currentState;
        }
        set
        {
            OnStateChange?.Invoke(currentState, value);
            currentState = value;
        }
    }
    private Coroutine currentCoroutine;
    protected IDamageable _target;

    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent OnStateChange;

    protected float _attackCooldown;
    protected float _speedMultiplier;
    protected float _idleUpdateRate;
    protected float _chaseUpdateRate;
    protected float _walkRadius;

    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetupAgentFromConfiguration();

        OnStateChange += HandleStateChange;
        OnStateChange?.Invoke(EnemyState.Spawn, defaultState);

    }

    // Manage attack state switch when damaged!!!
    protected abstract IEnumerator OnAttack();
    protected abstract IEnumerator OnIdle();
    protected abstract IEnumerator OnChase();
    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (oldState != newState)
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            switch (newState)
            {
                case EnemyState.Idle:
                    currentCoroutine = StartCoroutine(OnIdle());
                    break;
                case EnemyState.Chase:
                    currentCoroutine = StartCoroutine(OnChase());
                    break;
                case EnemyState.Attack:
                    currentCoroutine = StartCoroutine(OnAttack());
                    break;
            }
        }
    }
    public virtual void OnEnable()
    {
        followPlayerSensor.OnPlayerEnter += FollowPlayer_PlayerEnter;
        followPlayerSensor.OnPlayerExit += FollowPlayer_PlayerExit;
        attackPlayerSensor.OnPlayerEnter += AttackPlayer_PlayerEnter;
        attackPlayerSensor.OnPlayerExit += AttackPlayer_PlayerExit;
        // In case i will be doing object pool or something else
        //SetupAgentFromConfiguration();
    }

    protected virtual void OnDisable()
    {
        currentState = defaultState;
    }

    private void FollowPlayer_PlayerExit(Vector3 lastKnownPosition)
    {
        Agent.speed /= _speedMultiplier;
        State = EnemyState.Idle;
        _player = null;
    }

    private void FollowPlayer_PlayerEnter(Transform player)
    {
        Agent.speed *= _speedMultiplier;
        State = EnemyState.Chase;
        _player = player;
        _target = player.GetComponent<IDamageable>();
    }



    private void AttackPlayer_PlayerEnter(Transform player)
    {
        Agent.isStopped = true;
        _player = player;
        State = EnemyState.Attack;
    }

    private void AttackPlayer_PlayerExit(Vector3 lastKnownPosition)
    {
        Agent.isStopped = false;
        _player = null;
        State = EnemyState.Chase;
    }

    //private IEnumerator LookAt(Transform target)
    //{
    //    Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position - Vector3.up * 2);
    //    float time = 0;

    //    while (time < 1)
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
    //        time += Time.deltaTime * 2;
    //        yield return null;
    //    }
    //    transform.rotation = lookRotation;
    //}

    protected virtual void SetupAgentFromConfiguration()
    {
        // NavMesh Stats
        Agent.acceleration = EnemyScriptableObject.Acceleration;
        Agent.angularSpeed = EnemyScriptableObject.AngularSpeed;
        Agent.areaMask = EnemyScriptableObject.AreaMask;
        Agent.avoidancePriority = EnemyScriptableObject.AvoidancePriority;
        Agent.baseOffset = EnemyScriptableObject.BaseOffset;
        Agent.height = EnemyScriptableObject.Height;
        Agent.obstacleAvoidanceType = EnemyScriptableObject.ObstacleAvoidanceType;
        Agent.radius = EnemyScriptableObject.Radius;
        Agent.speed = EnemyScriptableObject.Speed;
        Agent.stoppingDistance = EnemyScriptableObject.StoppingDistance;


        // Enemy Stats
        Health = EnemyScriptableObject.Health;
        Damage = EnemyScriptableObject.Damage;
        _walkRadius = EnemyScriptableObject.WalkRadius;
        _attackCooldown = EnemyScriptableObject.AttackDelay;
        _idleUpdateRate = EnemyScriptableObject.IdleUpdateRate;
        _chaseUpdateRate = EnemyScriptableObject.AIUpdateInterval;
        _speedMultiplier = EnemyScriptableObject.SpeedMultiplier;
    }

    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
