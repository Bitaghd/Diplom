using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health = 300;

    private Coroutine LookCoroutine;

    private void Awake()
    {
        //_attackRadius.OnAttack += OnAttack;
    }


    public void TakeDamage(int damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
