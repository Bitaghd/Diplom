using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DogEnemy : Enemy
{
    protected override IEnumerator OnAttack()
    {
        animator.SetBool("isDogInAttackRange", true);
        WaitForSeconds wait = new WaitForSeconds(_attackCooldown);
        while (true)
        {
            transform.LookAt(_player);
            _target.TakeDamage(Damage);
            //Debug.Log("Otag");
            yield return wait;
        }
    }

    protected override IEnumerator OnChase()
    {
        WaitForSeconds wait = new WaitForSeconds(_chaseUpdateRate);
        animator.SetBool("isDogMoving", true);
        animator.SetBool("isDogInAttackRange", false);
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
        animator.SetBool("isDogInAttackRange", false);
        //Debug.Log(_idleUpdateRate);
        while (true)
        {
            animator.SetBool("isDogMoving", true);
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
            animator.SetBool("isDogMoving", false);
            yield return wait;
        }
    }
}
