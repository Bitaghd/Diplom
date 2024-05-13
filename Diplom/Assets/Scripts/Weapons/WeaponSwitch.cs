using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] private PlayerGunSelector _selector;
    [SerializeField] private Transform[] _weapons;
    [SerializeField] private int _weaponCount;
    [SerializeField] private KeyCode[] _keys;

    [SerializeField] private float _switchTime;

    private int _selectedWeapon;
    private float _timeSinceLastSwitch;
    private int _previousSelectedWeapon;

    private void Start()
    {
        SetWeapon();
        Select(_selectedWeapon);

        _timeSinceLastSwitch = 0f;
    }

    private void Update()
    {
        _previousSelectedWeapon = _selectedWeapon;

        for(int i = 0; i < _keys.Length; i++)
        {
            if (Input.GetKeyDown(_keys[i]) && _timeSinceLastSwitch >= _switchTime)
            {
                _selectedWeapon = i;
            }
        }

        if(_previousSelectedWeapon != _selectedWeapon)
        {
            Select(_selectedWeapon);
        }
        _timeSinceLastSwitch += Time.deltaTime;
    }

    private void Select(int weaponIndex)
    {
        for(int i = 0; i< _weapons.Length; i++)
        {
            _weapons[i].gameObject.SetActive(i == weaponIndex);
        }

        _timeSinceLastSwitch = 0;
    }


    private void SetWeapon()
    {
        _weapons = new Transform[_weaponCount];

        for(int i = 0; i <_weaponCount; i++)
        {
            _weapons[i] = transform.GetChild(i);
        }
        //_weapons = new Transform[transform.childCount];

        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    _weapons[i] = transform.GetChild(i);
        //}

        if (_keys == null)
        {
            _keys = new KeyCode[_weapons.Length];
        }
    }
}
