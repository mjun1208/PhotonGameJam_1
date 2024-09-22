using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum InventoryItemType
{
    None = -1,
    
    Shovel = 0,
    SeedBag_Corn = 1,
    FishRod = 2,
    Fish = 3,
    Axe = 4,
    
    BonFire = 5,
    Log = 6,
    CornSoup = 7,
    BlueCornBread = 8,
    SharkJuice = 9,
    
    Table = 10,
    Corn = 11,
    Plate = 12,
    Cup = 13,
    Chicken = 14,
    
    Pig = 15,
    Cow = 16,
    Egg = 17,
    Axe_1 = 18,
    Axe_2 = 19,
}

public class InventoryListItem : MonoBehaviour
{
    [SerializeField] private GameObject _selectImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private List<Sprite> _itemIcons;
    [SerializeField] private InventoryListItemDrag _drag;
    [SerializeField] private TMP_Text _itemCountText;
    
    private Player _player;
    private InventoryItemType _inventoryItemType = InventoryItemType.None;
    public InventoryUI InventoryUI { get; set; } = null;
    public InventoryBar InventoryBar { get; set; } = null;
    public int InventoryBarIndex { get; set; } = -1;
    
    public int ItemCount { get; set; } = 0;
    
    public bool Empty { get; set; } = true;
 
    public void Select(bool select)
    {
        _selectImage.SetActive(select);

        if (select)
        {
            SelectItemAction();
        }
    }

    public void SetInventoryItem(InventoryListItem inventoryListItem)
    {
        SetInventoryItemType(inventoryListItem.GetInventoryItemType, inventoryListItem.ItemCount);
    }
    
    public void SetInventoryItemType(InventoryItemType inventoryItemType, int itemCount)
    {
        _inventoryItemType = inventoryItemType;
        ItemCount = itemCount;
        
        if (inventoryItemType == InventoryItemType.None)
        {
            SetEmpty();
            return;
        }
        
        _iconImage.sprite = _itemIcons[(int)inventoryItemType];
        _iconImage.gameObject.SetActive(true);
        Empty = false;
        _itemCountText.text = $"{itemCount}";

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
        _itemCountText.text = "";
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

    public ChestInventoryItem ToChestInventoryItem()
    {
        return new ChestInventoryItem
        {
            e = this.Empty,
            t = (int)this._inventoryItemType,
            c = this.ItemCount,
        };
    }
}
