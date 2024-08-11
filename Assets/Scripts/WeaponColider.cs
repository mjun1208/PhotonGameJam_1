using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponColider : MonoBehaviour
{
    [SerializeField] private Weapon Weapon;
    
    private void OnTriggerStay(Collider other)
    {
        Weapon.aaOnTriggerStay(other);
    }
}
