using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ScriptableObject that holds the BASE STATS for an enemy. These can then be modified at object creation time to buff up enemies
/// and to reset their stats if they died or were modified at runtime
/// </summary>

[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScriptrableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    // Enemy stats
    public int Health = 100;
    public float AttackDelay = 1f;
    public int Damage = 5;
    public float AttackRadius = 1.5f;
    public bool IsRanged = false;
    public float SpeedMultiplier = 2f;
    public float IdleUpdateRate = 1.5f;
    public float WalkRadius = 4f;

    //NavMeshAgent Configs
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 8;
    public float AngularSpeed = 120;
    public int AreaMask = -1; // -1 means everything
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2.0f;
    public ObstacleAvoidanceType ObstacleAvoidanceType;
    public float Radius = 0.5f;
    public float Speed = 1.5f;
    public float StoppingDistance = 0.5f;

}
