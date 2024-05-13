using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class RobotEnemy : Enemy
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _lastTimeShoot = 0;
    //[SerializeField] TrailRenderer _trail;
    [SerializeField] TrailConfigurationScriptableObject TrailConfig;

    private ObjectPool<TrailRenderer> _trailPool;


    protected override void Awake()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        base.Awake();
    }
    protected override IEnumerator OnAttack()
    {
        animator.SetBool("isInAttackRange", true);
        // Delay for animation
        yield return new WaitForSeconds(_attackCooldown);
        while (true)
        {
            if (Time.time > _attackCooldown + _lastTimeShoot)
            {
                _lastTimeShoot = Time.time;

                //Vector3 erm = _player.transform.position.normalized;
                Vector3 shootDirection = _particleSystem.transform.forward;

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
                        PlayTrail(_particleSystem.transform.position, _particleSystem.transform.position + (shootDirection * TrailConfig.MissDistance), new RaycastHit())
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

    private void LookAt()
    {
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

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Neon Robot Trail");
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

    protected override IEnumerator OnChase()
    {
        WaitForSeconds wait = new WaitForSeconds(_chaseUpdateRate);
        animator.SetBool("isInAttackRange", false);
        animator.SetBool("isMoving", true);
        while (gameObject.activeSelf)
        {
            if (Agent.enabled)
            {
                if(_player != null)
                    Agent.SetDestination(_player.position);
            }
            yield return wait;
        }
    }

    protected override IEnumerator OnIdle()
    {
        WaitForSeconds wait = new WaitForSeconds(_idleUpdateRate);
        animator.SetBool("isInAttackRange", false);
        while (true)
        {
            animator.SetBool("isMoving", true);
            if (!Agent.enabled || !Agent.isOnNavMesh)
            {
                yield return wait;
            }
            else if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Vector3 randomDirection = Random.insideUnitSphere * _walkRadius;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(Agent.transform.position + randomDirection, out hit, _walkRadius, Agent.areaMask))
                {
                    Agent.SetDestination(hit.position);
                }

            }
            yield return wait;
            animator.SetBool("isMoving", false);
            yield return wait;
        }
    }
}
