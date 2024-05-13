using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private PlayerGunSelector _gunSelector;

    private void Update()
    {
        if (Input.GetMouseButton(0) && _gunSelector.ActiveGun != null)
        {
            _gunSelector.ActiveGun.Shoot();
        }
    }
}
