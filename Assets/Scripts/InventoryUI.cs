using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        }

        foreach (var inventoryListItem in _inventoryListItems)
        {
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

    public void OnBeginDrag(PointerEventData eventData, InventoryItemType inventoryItemType)
    {
        // _draggingItem
            
        if (!_draggingItem.gameObject.activeSelf)
        {
            _draggingItem.gameObject.SetActive(true);
        }

        if (_draggingItem.GetInventoryItemType != inventoryItemType)
        {
            _draggingItem.SetInventoryItemType(inventoryItemType);
        }

        _dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggingItem.gameObject.activeSelf)
        {
            _draggingItem.gameObject.SetActive(false);
        }

        _dragging = false;
    }

    public void CurrentItem()
    {
        
    }
}
