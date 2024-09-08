using Fusion;
using UnityEngine;

public partial class Player
{
    [SerializeField] private Log _log;
    [SerializeField] private GameObject _logFx;
    [SerializeField] private BonFire _bonfire;
    [SerializeField] private GameObject _bonFire_Ghost;
    [SerializeField] private GameObject _interactionText;
    
    private Tree _lookingTree;
    private Log _lookingLog;
    private BonFire _lookingBonfire;

    private void TreeUpdate(NetworkInputData inputData)
    {
        // if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        // {
        //     if (HasStateAuthority)
        //     {
        //         RpcTriggerRig();
        //     }
        //     
        //     _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
        // }
        
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            if (HasStateAuthority && _mouse0delay.ExpiredOrNotRunning(Runner))
            {
                if (_lookingBonfire != null)
                {
                    _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    return;
                }
                
                if (_shootAble)
                {
                    if (_shootType == ShootType.Bonfire)
                    {
                        _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                        SpawnBonFire();
                    }
                }
            }
        }
    }
    
    private void BonFireRender()
    {
        if (!HasInputAuthority)
        {
            return; 
        }
        
        if (!_bonFire_Ghost.activeSelf)
        {
            _bonFire_Ghost.SetActive(true);
        }

        _bonFire_Ghost.transform.position = _shootPosition;
    }

    public void SpawnBonFire()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        if (_shootAble && _shootType == ShootType.Bonfire)
        {
            Runner.Spawn(_bonfire, _shootPosition, Quaternion.LookRotation(_forward), Object.InputAuthority);
        }
    }

    public void SpawnLog(Vector3 hitPosition, Vector3 dir, Tree targetTree)
    {
        if (HasStateAuthority)
        {
            var lookPos = hitPosition + dir;
            
            var spawnedlog = Runner.Spawn(_log, hitPosition, Quaternion.LookRotation(lookPos), Object.InputAuthority);
            Instantiate(_logFx, hitPosition, Quaternion.LookRotation(lookPos));
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcTriggerRig()
    {
        _animator.SetTrigger("Rig");
        
        SetToolFalse();
        
        _axe.gameObject.SetActive(true);
    }
    
    private void GetBonFire()
    {
        LayerMask logLayer = 1 << LayerMask.NameToLayer("BonFire");

        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward,
                out RaycastHit hit, InteractionRayCastDistance))
        {
            var bonFire = hit.transform.GetComponent<BonFire>();
            if (bonFire)
            {
                if (_lookingBonfire != null && _lookingBonfire != bonFire)
                {
                    _lookingBonfire.Look(false);
                    _lookingBonfire = null;
                }

                bonFire.Look(true);
                _lookingBonfire = bonFire;
            }
            else
            {
                if (_lookingBonfire != null)
                {
                    _lookingBonfire.Look(false);
                    _lookingBonfire = null;
                }
            }
        }
        else
        {
            if (_lookingBonfire != null)
            {
                _lookingBonfire.Look(false);
                _lookingBonfire = null;
            }
        }

        if (_lookingBonfire != null)
        {
            _interactionText.gameObject.SetActive(true);
        }
        else
        {
            _interactionText.gameObject.SetActive(false);
        }
    }

    
    private void GetLog()
    {
        LayerMask logLayer = 1 << LayerMask.NameToLayer("Log");

        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward,
                out RaycastHit hit, InteractionRayCastDistance))
        {
            var log = hit.transform.GetComponent<Log>();
            if (log)
            {
                if (_lookingLog != null && _lookingLog != log)
                {
                    _lookingLog.Look(false);
                    _lookingLog = null;
                }

                log.Look(true);
                _lookingLog = log;
            }
            else
            {
                if (_lookingLog != null)
                {
                    _lookingLog.Look(false);
                    _lookingLog = null;
                }
            }
        }
        else
        {
            if (_lookingLog != null)
            {
                _lookingLog.Look(false);
                _lookingLog = null;
            }
        }
    }
    
    private void GetTree()
    {
        LayerMask treeLayer = 1 << LayerMask.NameToLayer("Tree");

        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward,
                out RaycastHit hit, InteractionRayCastDistance))
        {
            var tree = hit.transform.GetComponent<Tree>();
            if (tree)
            {
                if (_lookingTree != null && _lookingTree != tree)
                {
                    _lookingTree.Look(false);
                    _lookingTree = null;
                }

                tree.Look(true);
                _lookingTree = tree;
            }
            else
            {
                if (_lookingTree != null)
                {
                    _lookingTree.Look(false);
                    _lookingTree = null;
                }
            }
        }
        else
        {
            if (_lookingTree != null)
            {
                _lookingTree.Look(false);
                _lookingTree = null;
            }
        }
    }
}
