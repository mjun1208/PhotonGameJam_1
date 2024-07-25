using System;
using Fusion;
using UnityEngine;

public class KingDino : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    
    [Networked] private NetworkBool _isMove { get; set; }
    [Networked] private int _targetPlayerId { get; set; }

    private void Update()
    {
    }

    public override void FixedUpdateNetwork()
    {
    }

    public override void Render()
    {
    }

    private void GetTarget()
    {
        
    }
}
