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
        
        if (inputData.buttons.IsSet(NetworkInputData.MOUSEBUTTON0) && !LookingWho())
        {
            if (HasInputAuthority && _mouse0delay.ExpiredOrNotRunning(Runner) && CanClick)
            {
                if (_shootAble)
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

    private bool GiveItemToNpc()
    {
        bool giveItem = false;
        
        if (_lookingNpc != null)
        {
            foreach (var wantItemType in _lookingNpc.GetWantItemList())
            {
                if (_inventoryUI.GetInventoryItemCount(wantItemType) > 0)
                {
                    _lookingNpc.ReceiveItem(wantItemType);
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

                npc.Look(true);
                _lookingNpc = npc;
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
            if (_lookingBonfire == null)
            {
                _interactionText.transform.parent.gameObject.SetActive(false);
            }
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
}
