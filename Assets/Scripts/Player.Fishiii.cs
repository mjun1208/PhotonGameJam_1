using Fusion;
using UnityEngine;

public partial class Player
{
    [Networked] private int _attackState { get; set; } = 1;
    
    private int _mp = 15;

    private void FishWeaponUpdate(NetworkInputData inputData)
    {
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            // 공격..
            if (_isEquipWeapon)
            {
                if (_mouse0delay.ExpiredOrNotRunning(Runner))
                {
                    if (HasStateAuthority)
                    {
                        if (_attackState == 1)
                        {
                            _attackState = 2;

                            RpcTriggerAttack1();

                            SpendMp();
                            
                            _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                            return;
                        }

                        if (_attackState == 2)
                        {
                            _attackState = 3;

                            RpcTriggerAttack2();
                            
                            SpendMp();
                            
                            _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                            return;
                        }

                        if (_attackState == 3)
                        {
                            _attackState = 1;

                            RpcTriggerAttack3();

                            SpendMp();
                            
                            _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                            return;
                        }
                    }
                }

                return;
            }
            
            if (HasInputAuthority && _mouse0delay.ExpiredOrNotRunning(Runner))
            {
                if (_lookingFishWeapon != null)
                {
                    RpcEquipFishInputToState(_lookingFishWeapon);
                    _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                }
            }
        }
    }

    private void SpendMp()
    {
        _mp--;
        RpcSpendMp(_mp);
        
        if (_mp < 0)
        {
            RpcUnEquipFish2();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcSpendMp(int mp)
    {
        _HitCanvas.SetMp(mp);
    }

    public void HitOn()
    {
        _fishWeapon.HitOn(true);
        _fishWeapon.HitClear();
    }
    
    public void HitOff()
    {
        _fishWeapon.HitOn(false);
        _fishWeapon.HitClear();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcUnEquipFish2()
    {
        _isEquipWeapon = false;
        _fishWeapon.gameObject.SetActive(false);
        _fishWeapon2222.gameObject.SetActive(false);
        _fishRod.gameObject.SetActive(true);

        if (HasInputAuthority)
        {
            _HitCanvas.HideMp();
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcEquipFishInputToState(FishWeapon fishWeapon)
    {
        _isEquipWeapon = true;
        _mp = 15;
        
        Runner.Despawn(fishWeapon.GetComponent<NetworkObject>());
        
        RpcEquipFishStateToAll();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcEquipFishStateToAll()
    {
        _isEquipWeapon = true;
        _mp = 15;
        
        _fishWeapon.gameObject.SetActive(true);
        _fishRod.gameObject.SetActive(false);

        if (HasInputAuthority)
        {
            _fishWeapon.GetComponent<Renderer>().enabled = false;
            _fishWeapon.Effectss.SetActive(false);
            _fishWeapon2222.gameObject.SetActive(true);
            
            _HitCanvas.ShowMP(_mp);
        }
        
        _equipFx.gameObject.SetActive(true);
        _equipFx.Play();
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
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcTriggerAttack3()
    {
        _animator.SetTrigger("Attack3");
    }

    private FishWeapon _lookingFishWeapon = null;

    private void GetFish()
    {
        if (_isEquipWeapon)
        {
            if (_lookingFishWeapon != null)
            {
                _lookingFishWeapon.Look(false);
                _lookingFishWeapon = null;
            }

            return;
        }
        
        LayerMask fishMask = 1 << LayerMask.NameToLayer("Fish");

        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward,
                out RaycastHit hit, InteractionRayCastDistance))
        {
            var fishWeapon = hit.transform.GetComponent<FishWeapon>();
            if (fishWeapon)
            {
                if (_lookingFishWeapon != null && _lookingFishWeapon != fishWeapon)
                {
                    _lookingFishWeapon.Look(false);
                    _lookingFishWeapon = null;
                }

                fishWeapon.Look(true);
                _lookingFishWeapon = fishWeapon;
            }
            else
            {
                if (_lookingFishWeapon != null)
                {
                    _lookingFishWeapon.Look(false);
                    _lookingFishWeapon = null;
                }
            }
        }
        else
        {
            if (_lookingFishWeapon != null)
            {
                _lookingFishWeapon.Look(false);
                _lookingFishWeapon = null;
            }
        }
    }
}
