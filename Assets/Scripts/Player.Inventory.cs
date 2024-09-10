using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public partial class Player
{
    [SerializeField] private InventoryBar _inventoryBar;
    private InventoryItemType _inventoryItemType { get; set; }

    // public void OnChangedEquipItem()
    // {
    //     Equip(_inventoryItemType);
    // }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcEquipToServer(InventoryItemType inventoryItemType)
    {
        RpcEquipToAll(inventoryItemType);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcEquipToAll(InventoryItemType inventoryItemType)
    {
        Equip(inventoryItemType);
    }
    
    public void Equip(InventoryItemType inventoryItemType)
    {
        if (_inventoryItemType != inventoryItemType)
        {
            _inventoryItemType = inventoryItemType;
        }

        SetToolFalse();

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
            case InventoryItemType.Log:
            {
                break;
            }
            case InventoryItemType.Dummy_6:
            case InventoryItemType.Dummy_7:
            case InventoryItemType.Dummy_8:
            {
                break;
            }
        }
    }
}
