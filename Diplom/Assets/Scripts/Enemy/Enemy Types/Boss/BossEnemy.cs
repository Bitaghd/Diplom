using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossEnemy : Enemy, IDamageable
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] float time = 0;
    [SerializeField] private float _lastTimeShoot = 0;
    //[SerializeField] TrailRenderer _trail;
    [SerializeField] TrailConfigurationScriptableObject TrailConfig;

    private ObjectPool<TrailRenderer> _trailPool;

    [Header("Debug")]
    public int currentHealth;
    private Vector3 shootDirection;
    public static Action OnTakeDamage;

    protected override void Awake()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        base.Awake();
        currentHealth = Health;
    }
    protected override IEnumerator OnAttack()
    {
        while (true)
        {
            if (Time.time > _attackCooldown + _lastTimeShoot)
            {
                _lastTimeShoot = Time.time;

                shootDirection = (_player.position - _particleSystem.transform.position).normalized;

                if (Physics.Raycast(_particleSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, _layerMask))
                {

                    if (hit.transform.TryGetComponent(out IDamageable target))
                    {
                        target.TakeDamage(Damage);
                    }
                    StartCoroutine(PlayTrail(_particleSystem.transform.position, hit.point, hit));
                }
                else
                {
                    StartCoroutine(
                        PlayTrail(_particleSystem.transform.position, shootDirection, new RaycastHit())
                        );
                }
            }
            LookAt();
            yield return null;
        }
    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer instance = _trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPoint;
        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        _trailPool.Release(instance);
    }


    protected override IEnumerator OnChase()
    {
        while (true)
        {
            LookAt();
            yield return null;
        }

    }
    private void LookAt()
    {
        if (_player == null)
            return;
        Vector3 directionToPlayer = _player.position - transform.position;

        // Ignore the y-axis to avoid tilting
        directionToPlayer.y = 0f;

        if (directionToPlayer != Vector3.zero)
        {
            // Calculate rotation to look at the player
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

            // Apply the rotation to the NavMeshAgent
            Agent.transform.rotation = Quaternion.Slerp(Agent.transform.rotation, lookRotation, Time.deltaTime * 10);
        }
    }

    protected override IEnumerator OnIdle()
    {
        //WaitForSeconds wait = new WaitForSeconds(_idleUpdateRate);

        while (true)
        {
            time = 0;
            Quaternion lookRotation = Quaternion.Euler(0, UnityEngine.Random.Range(10, 90), 0);
            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
                time += Time.deltaTime * (_idleUpdateRate / 10);
                yield return null;
            }
            transform.rotation = lookRotation;
            //yield return wait;
            //transform.rotation = lookRotation;
            //yield return new WaitUntil(() => transform.rotation == lookRotation);
        }
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Neon Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }


    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
        OnTakeDamage?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(_particleSystem.transform.position, shootDirection, Color.red);
    }


}
