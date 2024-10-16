using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using UnityEngine;

public partial class Player
{
    [SerializeField] private Log _log;
    [SerializeField] private GameObject _logFx;
    [SerializeField] private GameObject _meatFx;
    [SerializeField] private BonFire _bonfire;
    [SerializeField] private GameObject _bonFire_Ghost;
    [SerializeField] private TMP_Text _interactionText;
    
    private Tree _lookingTree;
    private Log _lookingLog;
    private BonFire _lookingBonfire;

    private void TreeUpdate(NetworkInputData inputData)
    {
        if ((_inventoryItemType is InventoryItemType.Axe or InventoryItemType.Axe_1 or InventoryItemType.Axe_2) && !LookingWho())
        {
            if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                if (HasInputAuthority && CanClick)
                {
                    RpcTriggerRiga();
                }

                _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
            }
        }
        
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            if (HasInputAuthority && CanClick)
            {
                if (_lookingLog != null)
                {
                    // var networkObject = _lookingLog.GetComponent<NetworkObject>();
                    _lookingLog.Look(false);
                    RpcGetLog(_lookingLog);
                    _lookingLog = null;

                    // if (_inventoryUI.AddItem(InventoryItemType.Log, 1))
                    // {
                    //     _lookingLog.gameObject.SetActive(false);
                    //
                    //     var networkObject = _lookingLog.GetComponent<NetworkObject>();
                    //     RpcGetLogInputToState(networkObject);
                    //     
                    //     _lookingLog = null;
                    //     
                    //     _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    //     
                    //     // Tutorial
                    //     Global.Instance.IngameManager.ServerOnlyGameManager.TutorialManager.SetTutorialIndex(2);
                    // }
                }
            }
        }
        
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            if (HasInputAuthority && CanClick)
            {
                if (_lookingInteractItem != null)
                {
                    // var getItem = _lookingInteractItem.GetItem();
                    _lookingInteractItem.Look(false);
                    RpcGetItem(_lookingInteractItem);
                    _lookingInteractItem = null;
                    // if (_inventoryUI.AddItem(getItem.Item1, getItem.Item2))
                    // {
                    //     _lookingInteractItem.gameObject.SetActive(false);
                    //
                    //     var networkObject = _lookingInteractItem.GetComponent<NetworkObject>();
                    //     RpcGetLogInputToState(networkObject);
                    //     
                    //     _lookingInteractItem = null;
                    //     
                    //     _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    // }
                }
            }
        }

        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0) && !LookingWho())
        {
            if (HasInputAuthority && _mouse0delay.ExpiredOrNotRunning(Runner) && CanClick)
            {
                if (_lookingBonfire != null)
                {
                    _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    return;
                }
                
                if (_shootAble)
                {
                    if (_shootType == ShootType.Bonfire && _inventoryItemType == InventoryItemType.BonFire)
                    {
                        _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                        RpcSpawnBonFire(_shootPosition);
                        
                        _inventoryUI.RemoveItem(InventoryItemType.BonFire, 1);
                    }
                }
            }
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcGetItem(InteractItem interactItem)
    {
        if (interactItem != null)
        {
            var type = interactItem.GetItem();
            
            Global.Instance.MyPlayer.Runner.Despawn(interactItem.GetComponent<NetworkObject>());

            RpcGetItemToClient(type.Item1, type.Item2);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RpcGetItemToClient(InventoryItemType type, int count)
    {
        if (_inventoryUI.AddItem(type, count))
        {
            _pickUpSound.Play();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcGetLog(Log log)
    {
        if (log != null)
        {
            Global.Instance.MyPlayer.Runner.Despawn(log.GetComponent<NetworkObject>());

            RpcGetLogToClient();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RpcGetLogToClient()
    {
        if (_inventoryUI.AddItem(InventoryItemType.Log, 1))
        {
            // _lookingLog.gameObject.SetActive(false);
            // 
            // var networkObject = _lookingLog.GetComponent<NetworkObject>();
            // RpcGetLogInputToState(networkObject);

            // _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
            
            // Tutorial
            WaitAndTutorial();

            _pickUpSound.Play();
        }
    }

    private async void WaitAndTutorial()
    {
        await UniTask.NextFrame();
        Global.Instance.IngameManager.ServerOnlyGameManager.RpcSetTutorialIndex(2);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcGetLogInputToState(NetworkObject log)
    {
        Debug.Log(log.Id);
        Global.Instance.MyPlayer.Runner.Despawn(log);
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

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcSpawnBonFire(Vector3 shootPosition)
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        if (_shootAble && _shootType == ShootType.Bonfire)
        {
            Runner.Spawn(_bonfire, shootPosition, Quaternion.LookRotation(_forward), Object.StateAuthority);
        }
    }

    public void SpawnLog(Vector3 hitPosition, Vector3 dir, Tree targetTree)
    {
        var lookPos = hitPosition + dir;
        
        if (HasStateAuthority)
        {
            int count = 1;
            
            if (_inventoryItemType == InventoryItemType.Axe)
            {
                count = 1;
            }
            if (_inventoryItemType == InventoryItemType.Axe_1)
            {
                count = 3;
            }
            if (_inventoryItemType == InventoryItemType.Axe_2)
            {
                count = 5;
            }

            for (int i = 0; i < count; i++)
            {
                var spawnedlog = Runner.Spawn(_log, hitPosition, Quaternion.LookRotation(lookPos), Object.StateAuthority);   
            }

            // Tutorial
            Global.Instance.IngameManager.ServerOnlyGameManager.RpcSetTutorialIndex(1);
            
            // appleSpawn
            float rangeX = 3f; // x축 범위
            float rangeZ = 3f; // z축 범위

            float randomX = Random.Range(targetTree.AppleSpawnPoint.position.x - rangeX, targetTree.AppleSpawnPoint.position.x + rangeX);
            float randomZ = Random.Range(targetTree.AppleSpawnPoint.position.z - rangeZ, targetTree.AppleSpawnPoint.position.z + rangeZ);
            
            Runner.Spawn(targetTree.Apple, new Vector3(randomX, targetTree.AppleSpawnPoint.position.y, randomZ), 
                Quaternion.LookRotation(lookPos), Object.StateAuthority);   
        }
        
        targetTree.Hit();
        Instantiate(_logFx, hitPosition, Quaternion.LookRotation(lookPos));
    }
    
    public void SpawnMeat(Vector3 hitPosition, Vector3 dir, Animal targetAnimal)
    {
        var lookPos = hitPosition + dir;
        
        int count = 1;
            
        if (_inventoryItemType == InventoryItemType.Axe)
        {
            count = 1;
        }
        if (_inventoryItemType == InventoryItemType.Axe_1)
        {
            count = 2;
        }
        if (_inventoryItemType == InventoryItemType.Axe_2)
        {
            count = 3;
        }
        
        targetAnimal.Hit(count);
        Instantiate(_meatFx, hitPosition, Quaternion.LookRotation(lookPos));
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcTriggerRiga()
    {
        RpcTriggerRig();
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
            _interactionText.text = "클릭 - 제작하기";
            _interactionText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            DisableInteractionText();
        }
    }

    
    private void GetLog()
    {
        LayerMask logLayer = 1 << LayerMask.NameToLayer("Log");

        if (Physics.SphereCast(_playerCameraRootTransform.transform.position, 0.25f, _playerCameraRootTransform.transform.forward, out RaycastHit hit,
                InteractionRayCastDistance, logLayer))
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
