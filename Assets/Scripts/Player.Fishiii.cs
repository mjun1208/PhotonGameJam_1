using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public partial class Player
{
    [Networked] private int _attackState { get; set; } = 1;

    private void FishWeaponUpdate(NetworkInputData inputData)
    {
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            if (HasStateAuthority && _mouse0delay.ExpiredOrNotRunning(Runner))
            {
                _mouse0delay = TickTimer.CreateFromSeconds(Runner, 1f);

                if (_attackState == 1)
                {
                    _attackState = 2;

                    RpcTriggerAttack1();
                }
                else
                {
                    _attackState = 1;

                    RpcTriggerAttack2();
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcTriggerAttack1()
    {
        _animator.SetTrigger("Attack1");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcTriggerAttack2()
    {
        _animator.SetTrigger("Attack2");
    }
}
