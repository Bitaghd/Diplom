using UnityEngine;

public interface IDamageable
{
    //public int CurrentHealth { get; }
    //public int MaxHealth { get; }

    //public delegate void TakeDamageEvent(int damage);
    //public event TakeDamageEvent OnTakeDamage;

    //public delegate void DeathEvent(Vector3 position);
    //public event DeathEvent OnDeath;
    void TakeDamage(int damage);
    Transform GetTransform();
}
