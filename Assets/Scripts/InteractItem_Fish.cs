using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractItem_Fish : InteractItem
{
    [SerializeField] private Rigidbody _rigidbody;
    
    public void Fished()
    {
        _rigidbody.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }
}
