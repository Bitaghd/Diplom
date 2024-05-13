using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PlayerSensor : MonoBehaviour
{
    public delegate void PlayerEnterEvent(Transform player);
    public delegate void PlayerExitEvent(Vector3 lastKnownPosition);

    public event PlayerEnterEvent OnPlayerEnter;
    public event PlayerExitEvent OnPlayerExit;

    private void OnTriggerEnter(Collider other)
    {
        // Check how it interacts with other enemies
        if (other.TryGetComponent(out IDamageable player))
        {
            OnPlayerEnter?.Invoke(player.GetTransform());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check how it interacts with other enemies
        if (other.TryGetComponent(out IDamageable player))
        {
            OnPlayerExit?.Invoke(other.transform.position);
        }
    }
}
