using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryUI : MonoBehaviour
{
    public enum InventoryTab
    {
        Inventory,
        Craft,
    }

    [SerializeField] private Image _inventoryTabButton;
    [SerializeField] private Image _craftTabButton;
    
    [SerializeField] private InventoryListItem _draggingItem;
    private bool _dragging = false;
    private InventoryItemType _draggingInventoryItemType;
    private InventoryListItem _dragSlot;
    
    [SerializeField] private GameObject _inventoryGroup;
    [SerializeField] private GameObject _craftGroup;
    [SerializeField] private InventoryBar _inventoryBar;

    [SerializeField] private List<InventoryListItem> _inventoryListItems;
    [SerializeField] private List<InventoryListItem> _inventoryBarListItems;
    
    private InventoryTab _inventoryTab = InventoryTab.Inventory;

    // private InventoryListItem _draggingItem;

    public void OnClickInventoryTab()
    {
        _inventoryTab = InventoryTab.Inventory;
        
        _inventoryTabButton.color = Color.white;
        _craftTabButton.color = Color.gray;
        
        _inventoryGroup.SetActive(true);
        _craftGroup.SetActive(false);

        for (int i = 0; i < _inventoryBar.InventoryListItems.Count; i++)
        {
            _inventoryBarListItems[i].SetInventoryItemType(_inventoryBar.InventoryListItems[i].GetInventoryItemType);
            _inventoryBarListItems[i].SetInventoryUI(this);
            _inventoryBarListItems[i].SetInventoryBar(_inventoryBar);
            _inventoryBarListItems[i].SetInventoryBarIndex(i);
        }

        foreach (var inventoryListItem in _inventoryListItems)
        {
            inventoryListItem.SetInventoryItemType((InventoryItemType)Random.Range(0, (int)InventoryItemType.Dummy_8));
            inventoryListItem.SetInventoryUI(this);
        }

        _dragging = false;
    }
    
    public void OnClickCraftTab()
    {
        _inventoryTab = InventoryTab.Craft;
        
        _craftTabButton.color = Color.white;
        _inventoryTabButton.color = Color.gray;
        
        _craftGroup.SetActive(true);
        _inventoryGroup.SetActive(false);
    }

    private void Update()
    {
        if (_dragging)
        {
            if (!_draggingItem.gameObject.activeSelf)
            {
                _draggingItem.gameObject.SetActive(true);
            }

            _draggingItem.transform.position = Input.mousePosition;
        }
        else
        {
            if (_draggingItem.gameObject.activeSelf)
            {
                _draggingItem.gameObject.SetActive(false);
            }
        }
    }

    public InventoryItemType GetDraggingItemType => _draggingInventoryItemType;

    public void OnBeginDrag(PointerEventData eventData, InventoryListItem slot, InventoryItemType inventoryItemType)
    {
        // _draggingItem

        if (_dragging)
        {
            return;
        }

        _dragSlot = slot;
            
        if (!_draggingItem.gameObject.activeSelf)
        {
            _draggingItem.gameObject.SetActive(true);
        }

        if (_draggingItem.GetInventoryItemType != inventoryItemType)
        {
            _draggingItem.SetInventoryItemType(inventoryItemType);
        }

        _dragging = true;

        _draggingInventoryItemType = inventoryItemType;
    }

    public void OnDrop()
    {
        if (_draggingItem.gameObject.activeSelf)                                                         
        {
            _draggingItem.gameObject.SetActive(false);
        }
        
        _dragSlot.SetInventoryItemType(GetDraggingItemType);

        _dragging = false;
    }
    
    public void OnDrop(PointerEventData eventData, InventoryItemType inventoryItemType, bool isEmpty)
    {
        if (_draggingItem.gameObject.activeSelf)
        {
            _draggingItem.gameObject.SetActive(false);
        }
        
        _dragSlot.SetInventoryItemType(inventoryItemType);
        if (isEmpty)
        {
            _dragSlot.SetEmpty();
        }
        
        if (_dragSlot.InventoryBar != null)
        {
            _inventoryBar.InventoryListItems[_dragSlot.InventoryBarIndex].SetInventoryItemType(_dragSlot.GetInventoryItemType);

            if (_dragSlot.InventoryBar.CurrentIndex == _dragSlot.InventoryBarIndex)
            {
                _inventoryBar.SelectItem(_dragSlot.InventoryBar.CurrentIndex);
            }
        }

        _dragging = false;
    }

    public void CurrentItem()
    {
        
    }
}
