using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryItemType
{
    Shovel,
    SeedBag,
    FishRod,
    Fish,
    Axe,
    Log,
    Dummy_6,
    Dummy_7,
    Dummy_8,
}

public class InventoryListItem : MonoBehaviour
{
    [SerializeField] private GameObject _selectImage;
    private Player _player;
    private InventoryItemType _inventoryItemType;

    public void Select(bool select)
    {
        _selectImage.SetActive(select);

        if (select)
        {
            SelectItemAction();
        }
    }

    public void SetInventoryItemType(InventoryItemType inventoryItemType)
    {
        _inventoryItemType = inventoryItemType;
    }

    private void SelectItemAction()
    {
        _player.RpcEquipToServer(_inventoryItemType);
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }
}
