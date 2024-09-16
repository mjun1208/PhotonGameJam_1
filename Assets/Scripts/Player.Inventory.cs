using Fusion;
using UnityEngine;

public partial class Player
{
    [SerializeField] private InventoryBar _inventoryBar;
    [SerializeField] private InventoryUI _inventoryUI;
    private InventoryItemType _inventoryItemType { get; set; }
    
    [Networked] private NetworkBool _isInventoryOpen { get; set; } = false;

    // public void OnChangedEquipItem()
    // {
    //     Equip(_inventoryItemType);
    // }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcOpenInventoryUI(bool isOpen)
    {
        _isInventoryOpen = isOpen;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcEquipToServer(InventoryItemType inventoryItemType, bool isEmpty)
    {
        Equip(inventoryItemType, isEmpty);
        
        RpcEquipToAll(inventoryItemType, isEmpty);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcEquipToAll(InventoryItemType inventoryItemType, bool isEmpty)
    {
        Equip(inventoryItemType, isEmpty);
    }
    
    public void Equip(InventoryItemType inventoryItemType, bool isEmpty)
    {
        if (_inventoryItemType != inventoryItemType)
        {
            _inventoryItemType = inventoryItemType;
        }

        if (inventoryItemType == InventoryItemType.Table)
        {
            if (HasInputAuthority)
            {
                Global.Instance.IngameManager.IsTabling = true;
                Global.Instance.IngameManager.Tables.ForEach(x=> x.ShowSizeObject(true));
            }
        }
        else
        {
            if (HasInputAuthority)
            {
                Global.Instance.IngameManager.IsTabling = false;
                Global.Instance.IngameManager.Tables.ForEach(x=> x.ShowSizeObject(false));
            }
        }

        SetToolFalse();

        if (isEmpty || inventoryItemType == InventoryItemType.None)
        {
            _inventoryItemType = InventoryItemType.None;
            return;
        }

        switch (_inventoryItemType)
        {
            case InventoryItemType.Shovel:
            {
                _shovel.SetActive(true);
                break;
            }
            case InventoryItemType.SeedBag:
            {
                _seedBag.SetActive(true);
                break;
            }
            case InventoryItemType.FishRod:
            {
                _fishRod.SetActive(true);
                break;
            }
            case InventoryItemType.Fish:
            {
                // _fishWeapon.gameObject.SetActive(true);
                // _fishWeapon2222.gameObject.SetActive(true);
                break;
            }
            case InventoryItemType.Axe:
            {
                _axe.gameObject.SetActive(true);   
                break;
            }
            case InventoryItemType.BonFire:
            {
                break;
            }
            case InventoryItemType.Log:
            {
                break;
            }
            case InventoryItemType.Table:
            {
                break;
            }
            default:
            {
                break;
            }
        }
    }
}
