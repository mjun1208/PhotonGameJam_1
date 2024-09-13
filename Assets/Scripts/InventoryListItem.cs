using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [SerializeField] private Image _iconImage;
    [SerializeField] private List<Sprite> _itemIcons;
    [SerializeField] private InventoryListItemDrag _drag;
    private Player _player;
    private InventoryItemType _inventoryItemType;
    public InventoryUI InventoryUI { get; set; } = null;
    public InventoryBar InventoryBar { get; set; } = null;
    public int InventoryBarIndex { get; set; } = -1;
    
    public bool Empty { get; set; } = true;
 
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
        _iconImage.sprite = _itemIcons[(int)inventoryItemType];
        _iconImage.gameObject.SetActive(true);
        Empty = false;

        if (_drag != null)
        {
            _drag.gameObject.SetActive(true);
        }
    }

    public void SetInventoryUI(InventoryUI inventoryUI)
    {
        InventoryUI = inventoryUI; 
    }
    
    public void SetInventoryBar(InventoryBar inventoryBar)
    {
        InventoryBar = inventoryBar; 
    }
    
    public void SetInventoryBarIndex(int index)
    {
        InventoryBarIndex = index;
    }

    public InventoryItemType GetInventoryItemType => _inventoryItemType;

    public void SetEmpty(bool dragging = false)
    {
        _iconImage.gameObject.SetActive(false);
        Empty = true;

        if (!dragging)
        {
            if (_drag != null)
            {
                _drag.gameObject.SetActive(false);
            }
        }
    }

    private void SelectItemAction()
    {
        _player.RpcEquipToServer(_inventoryItemType, Empty);
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public bool Dragging { get; set; } = false;
}
