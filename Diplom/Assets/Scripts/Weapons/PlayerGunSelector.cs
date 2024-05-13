using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField] private GunType _gun;
    [SerializeField] private Transform _gunParent;
    [SerializeField] private List<GunScriptableObject> _guns;
    //[SerializeField] private PlayerIK InverseKinematics;

    [Space]
    [Header("Runtime Filled")]
    public GunScriptableObject ActiveGun;

    private void Start()
    {
        GunScriptableObject gun = _guns.Find(gun => gun.Type == _gun);
        if(gun == null)
        {
            Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
            return;
        }

        ActiveGun = gun;
        gun.Spawn(_gunParent, this);

        //Inverse kinematics shit
    }
}
