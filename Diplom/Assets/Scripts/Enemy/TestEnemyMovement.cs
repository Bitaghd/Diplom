using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TestEnemyMovement : MonoBehaviour
{
    public float UpdateRate = 0.1f;
    public Transform Player;

    private NavMeshAgent _agent;

    private Coroutine _followCoroutine;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void StartChasing()
    {
        if (_followCoroutine == null)
        {
            StartCoroutine(FollowTarget());
        }
        else
            Debug.LogWarning("Called StartChasing on Enemy that is already chasing! This is likely a bug in some calling class!");
        
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(UpdateRate);

        while(enabled)
        {
            _agent.SetDestination(Player.transform.position - (Player.transform.position - transform.position).normalized * 0.5f);
            //transform.LookAt(_target);
            yield return wait;
        }
        
    }
}
