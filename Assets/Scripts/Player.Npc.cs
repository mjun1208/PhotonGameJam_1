using Fusion;
using UnityEngine;

public partial class Player
{
    [SerializeField] private Table _table;
    [SerializeField] private Renderer _table_Ghost;

    private Color _tableYesColor = new Color(0, 248f / 255f, 3f / 255f, 1f);
    private Color _tableNoColor = new Color(248 / 255f, 0f, 3f / 255f, 1f);
    private bool _tableYes = false;
    
    private NPC _lookingNpc = null;
    private Table _lookingTable = null;
    private Shop _lookingShop = null;

    private void NpcUpdate(NetworkInputData inputData)
    {
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            if (HasInputAuthority && CanClick)
            {
                if (_lookingNpc != null)
                {
                    if (!GiveItemToNpc())
                    {
                        ShowNotice("건네줄 수 있는 아이템이 없습니다.", Color.red);
                    }

                    _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                }
            }
        }
        
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            if (HasInputAuthority && CanClick)
            {
                if (_lookingShop != null)
                {
                    _inventoryUI.gameObject.SetActive(!_inventoryUI.gameObject.activeSelf);

                    RpcOpenInventoryUI(_inventoryUI.gameObject.activeSelf);

                    if (_inventoryUI.gameObject.activeSelf)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        _inventoryUI.OnClickShop();

                        Global.Instance.IngameActivingCursor = true;
                    }
                    
                    _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                }
            }
        }
        
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            if (HasInputAuthority && CanClick)
            {
                if (_lookingTable != null)
                {
                    RpcReceiveReward(_lookingTable);
                    _mouse0delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                }
            }
        }
        
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0) && !LookingWho())
        {
            if (HasInputAuthority && _mouse0delay.ExpiredOrNotRunning(Runner) && CanClick)
            {
                if (_shootAble && _inventoryItemType == InventoryItemType.Table)
                {
                    if (_shootType == ShootType.Table)
                    {
                        if (!_tableYes)
                        {
                            ShowNotice("설치 할 수 없는 공간입니다.", Color.red);
                            return;
                        }
                        
                        _mouse0delay = TickTimer.CreateFromSeconds(Runner, 1f);
                        RpcSpawnTable(_shootPosition, _table_Ghost.transform.rotation);
                        
                        _inventoryUI.RemoveItem(InventoryItemType.Table, 1);
                    }
                }
            }
        }
    }
        
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcReceiveReward(Table lookingTable)
    {
        lookingTable.ReceiveReward();
    }
    
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcGiveItemToNpc(NPC lookingNpc, InventoryItemType type)
    {
        lookingNpc.ReceiveItem(type);
    }

    private bool GiveItemToNpc()
    {
        bool giveItem = false;
        
        if (_lookingNpc != null)
        {
            foreach (var wantItemType in _lookingNpc.GetWantItemList())
            {
                if (_inventoryUI.GetInventoryItemCount(wantItemType) > 0)
                {
                    RpcGiveItemToNpc(_lookingNpc, wantItemType);
                    _inventoryUI.RemoveItem(wantItemType, 1);
                    giveItem = true;
                }
            }
        }

        return giveItem;
    }
    
    private void GetNpc()
    {
        LayerMask logLayer = 1 << LayerMask.NameToLayer("Npc");

        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward,
                out RaycastHit hit, InteractionRayCastDistance))
        {
            var npc = hit.transform.GetComponent<NPC>();
            if (npc)
            {
                if (_lookingNpc != null && _lookingNpc != npc)
                {
                    _lookingNpc.Look(false);
                    _lookingNpc = null;
                }

                if (!npc.IsEnd && npc.IsStart)
                {
                    npc.Look(true);
                    _lookingNpc = npc;
                }
                else
                {
                    npc.Look(false);
                    _lookingNpc = null;
                }
            }
            else
            {
                if (_lookingNpc != null)
                {
                    _lookingNpc.Look(false);
                    _lookingNpc = null;
                }
            }
        }
        else
        {
            if (_lookingNpc != null)
            {
                _lookingNpc.Look(false);
                _lookingNpc = null;
            }
        }

        if (_lookingNpc != null)
        {
            _interactionText.text = "클릭 - 건네주기";
            _interactionText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            DisableInteractionText();
        }
    }
    
    private void TableRender(bool tableYes)
    {
        if (!HasInputAuthority)
        {
            return; 
        }
        
        if (!_table_Ghost.gameObject.activeSelf)
        {
            _table_Ghost.gameObject.SetActive(true);
        }

        if (_tableYes != tableYes)
        {
            var newMaterial = new Material(_table_Ghost.material);
            
            if (tableYes)
            {
                newMaterial.SetColor("_Ghost_Color", _tableYesColor);
            }
            else
            {
                newMaterial.SetColor("_Ghost_Color", _tableNoColor);
            }

            _table_Ghost.material = newMaterial;
        }

        _tableYes = tableYes;
        
        _table_Ghost.transform.position = _shootPosition;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcSpawnTable(Vector3 shootPosition, Quaternion shootRotation)
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        if (_shootAble && _shootType == ShootType.Table)
        {
            Runner.Spawn(_table, shootPosition, shootRotation, Object.StateAuthority);
        }
    }

    private void GetTable()
    {
        LayerMask tableLayer = 1 << LayerMask.NameToLayer("Table");
        
        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward,
                out RaycastHit hit, InteractionRayCastDistance, tableLayer))
        {
            var table = hit.transform.GetComponent<Table>();
            if (table)
            {
                if (_lookingTable != null && _lookingTable != table)
                {
                    _lookingTable.Look(false);
                    _lookingTable = null;
                }

                if (table.RewardCount > 0)
                {
                    table.Look(true);
                    _lookingTable = table;
                }
                else
                {
                    table.Look(false);
                    _lookingTable = null;
                }
            }
            else
            {
                if (_lookingTable != null)
                {
                    _lookingTable.Look(false);
                    _lookingTable = null;
                }
            }
        }
        else
        {
            if (_lookingTable != null)
            {
                _lookingTable.Look(false);
                _lookingTable = null;
            }
        }

        if (_lookingTable != null)
        {
            _interactionText.text = "클릭 - 보상 받기";
            _interactionText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            DisableInteractionText();
        }
    }
    
    private void GetShop()
    {
        if (Physics.Raycast(_playerCameraRootTransform.transform.position, _playerCameraRootTransform.transform.forward,
                out RaycastHit hit, InteractionRayCastDistance))
        {
            var shop = hit.transform.GetComponent<Shop>();
            if (shop)
            {
                if (_lookingShop != null && _lookingShop != shop)
                {
                    _lookingShop.Look(false);
                    _lookingShop = null;
                }

                shop.Look(true);
                _lookingShop = shop;
            }
            else
            {
                if (_lookingShop != null)
                {
                    _lookingShop.Look(false);
                    _lookingShop = null;
                }
            }
        }
        else
        {
            if (_lookingShop != null)
            {
                _lookingShop.Look(false);
                _lookingShop = null;
            }
        }

        if (_lookingShop != null)
        {
            _interactionText.text = "클릭 - 상점";
            _interactionText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            DisableInteractionText();
        }
    }
}
