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

public class InventoryListItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] private GameObject _rayCastImage;
    [SerializeField] private GameObject _selectImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private List<Sprite> _itemIcons;
    private Player _player;
    private InventoryItemType _inventoryItemType;
    private InventoryUI _inventoryUI;
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
    }

    public void SetInventoryUI(InventoryUI inventoryUI)
    {
        _inventoryUI = inventoryUI; 
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

    public void SetEmpty()
    {
        _iconImage.gameObject.SetActive(false);
        Empty = true;
    }

    private void SelectItemAction()
    {
        _player.RpcEquipToServer(_inventoryItemType);
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public bool Dragging { get; set; } = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Dragging = true;
        SetEmpty();

        _inventoryUI.OnBeginDrag(eventData, this, _inventoryItemType);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Dragging = false;
        
        GameObject hoveredObject = eventData.pointerCurrentRaycast.gameObject;

        if (hoveredObject != null && hoveredObject.GetComponent<InventoryListItem>() != null)
        {
            // 드래그 중인 슬롯 은 현재

            // 마우스 아래 슬롯
            InventoryListItem targetSlot = hoveredObject.GetComponent<InventoryListItem>();
            _inventoryUI.OnDrop(eventData, targetSlot.GetInventoryItemType, targetSlot.Empty);
            
            targetSlot.SetInventoryItemType(_inventoryUI.GetDraggingItemType);

            if (targetSlot.InventoryBar != null)
            {
                targetSlot.InventoryBar.InventoryListItems[targetSlot.InventoryBarIndex].SetInventoryItemType(targetSlot.GetInventoryItemType);
                if (targetSlot.InventoryBar.CurrentIndex == targetSlot.InventoryBarIndex)
                {
                    targetSlot.InventoryBar.SelectItem(targetSlot.InventoryBarIndex);
                }
            }
        }
        else
        {
            _inventoryUI.OnDrop(); 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Dragging = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }
}
