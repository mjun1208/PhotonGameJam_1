using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum InventoryItemType
{
    None = -1,
    
    Axe = 0,
    Shovel = 1,
    FishRod = 2,
    BonFire = 3,
    Table = 4,
    
    Axe_1 = 5,
    Axe_2 = 6,

    // 농 + 임
    Log = 7,
    Apple = 8,
    Corn = 9,
    Carrot = 10,
    Cola = 11,
    
    // 축 
    Chicken = 12,
    Pig = 13,
    Cow = 14,
    Egg = 15,
    
    // 수
    Fish = 16,
    Fish_Shark = 17,
    Turtle = 18,
    
    // 부재료
    Plate = 19,
    Cup = 20,
    
    // 씨앗
    SeedBag_Corn = 21,
    SeedBag_Carrot = 22,
    SeedBag_Cola = 23,
    
    // 음식 1 ~ 5
    CookedCorn = 24,
    CookedChicken = 25,
    FishWater = 26,
    AppleCarrotJuice = 27,
    CornPie = 28,
    
    // 6 ~ 10
    PigAndChicken = 29,
    TurtleAndFish = 30,
    VeganSet = 31,
    
    // 10 ~ 15
    BergurSet = 32,
    SharkRamen = 33,
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
            
            _inventoryItemType = InventoryItemType.None;
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
