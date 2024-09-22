using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class InventoryUI : MonoBehaviour
{
    public enum InventoryTab
    {
        Inventory,
        Craft,
        Shop,
    }

    [SerializeField] private Image _inventoryTabButton;
    [SerializeField] private Image _craftTabButton;
    
    [SerializeField] private InventoryListItem _draggingItem;
    private bool _dragging = false;
    private InventoryItemType _draggingInventoryItemType;
    private int _draggingInventoryItemCount;
    private InventoryListItem _dragSlot;
    
    [SerializeField] private GameObject _inventoryGroup;
    [SerializeField] private GameObject _craftGroup;
    [SerializeField] protected InventoryBar _inventoryBar;

    [SerializeField] public List<InventoryListItem> _inventoryListItems;
    [SerializeField] public List<InventoryListItem> _inventoryBarListItems;
    
    private InventoryTab _inventoryTab = InventoryTab.Inventory;

    // private InventoryListItem _draggingItem;

    public void SetUpInventory()
    {
        for (int i = 0; i < _inventoryBar.InventoryListItems.Count; i++)
        {
            _inventoryBarListItems[i].SetInventoryItemType(_inventoryBar.InventoryListItems[i].GetInventoryItemType, _inventoryBar.InventoryListItems[i].ItemCount);
            _inventoryBarListItems[i].SetInventoryUI(this);
            _inventoryBarListItems[i].SetInventoryBar(_inventoryBar);
            _inventoryBarListItems[i].SetInventoryBarIndex(i);

            if (_inventoryBar.InventoryListItems[i].Empty)
            {
                _inventoryBarListItems[i].SetEmpty();
            }
        }

        foreach (var inventoryListItem in _inventoryListItems)
        {
            // inventoryListItem.SetInventoryItemType((InventoryItemType)Random.Range(0, (int)InventoryItemType.Dummy_8));
            inventoryListItem.SetInventoryUI(this);
        }
    }

    public void OnClickInventoryTab()
    {
        _inventoryTab = InventoryTab.Inventory;
        
        _inventoryTabButton.color = Color.white;
        _craftTabButton.color = Color.gray;
        
        _shopGroup.SetActive(false);
        _invenGroup.SetActive(true);
        
        _inventoryGroup.SetActive(true);
        _craftGroup.SetActive(false);

        SetUpInventory();

        _dragging = false;
    }
    
    protected virtual void OnDisable()
    {
        if (_dragging)
        {
            _dragSlot.SetInventoryItem(_dragSlot);
            _dragSlot.Dragging = false;
            _dragging = false;
            
            _draggingItem.gameObject.SetActive(false);
        }
        
        if (_craftEnd != null)
        {
            _craftEnd.gameObject.SetActive(false);
        }
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
    public int GetDraggingItemCount => _draggingInventoryItemCount;
    public InventoryListItem GetDraggingItem => _draggingItem;

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

        _draggingItem.SetInventoryItem(_dragSlot);

        _dragging = true;

        _draggingInventoryItemType = inventoryItemType;
    }

    public void OnDrop()
    {
        if (_draggingItem.gameObject.activeSelf)                                                         
        {
            _draggingItem.gameObject.SetActive(false);
        }
        
        _dragSlot.SetInventoryItem(GetDraggingItem);

        _dragging = false;
    }
    
    public void OnDrop(PointerEventData eventData, InventoryListItem inventoryItem, bool isEmpty)
    {
        if (_draggingItem.gameObject.activeSelf)
        {
            _draggingItem.gameObject.SetActive(false);
        }

        // var origin = _dragSlot.GetInventoryItemType;
        _dragSlot.SetInventoryItem(inventoryItem);
        if (isEmpty)
        {
            _dragSlot.SetEmpty();
        }
        
        if (_dragSlot.InventoryBar != null)
        {
            _inventoryBar.InventoryListItems[_dragSlot.InventoryBarIndex].SetInventoryItem(_draggingItem);
            
            if (isEmpty)
            {
                _inventoryBar.InventoryListItems[_dragSlot.InventoryBarIndex].SetEmpty();
            }
            
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

    // 없으면 비어있는 칸 제일 첫번째를 리턴
    // null 이면 들어갈 수 있는 곳 없음
    public InventoryListItem GetFirstItemSlot(InventoryItemType type)
    {
        InventoryListItem firstListItem = null;
        InventoryListItem emptyFirstListItem = null;
        
        foreach (var inventoryBarListItem in _inventoryBarListItems)
        {
            if (!inventoryBarListItem.Empty)
            {
                if (inventoryBarListItem.GetInventoryItemType == type && firstListItem == null)
                {
                    firstListItem = inventoryBarListItem;
                    break;
                }
            }
            else
            {
                if (emptyFirstListItem == null)
                {
                    emptyFirstListItem = inventoryBarListItem;
                }
            }
        }

        if (firstListItem == null)
        {
            foreach (var inventoryListItem in _inventoryListItems)
            {
                if (!inventoryListItem.Empty)
                {
                    if (inventoryListItem.GetInventoryItemType == type && firstListItem == null)
                    {
                        firstListItem = inventoryListItem;
                        break;
                    }
                }
                else
                {
                    if (emptyFirstListItem == null)
                    {
                        emptyFirstListItem = inventoryListItem;
                    }
                }
            }
        }

        if (firstListItem == null)
        {
            firstListItem = emptyFirstListItem;
        }

        return firstListItem;
    }

    public bool AddItem(InventoryItemType inventoryItemType, int count)
    {
        var slot = GetFirstItemSlot(inventoryItemType);
        if (slot != null)
        {
            if (slot.Empty)
            {
                slot.ItemCount = count;
            }
            else
            {
                slot.ItemCount += count;
            }

            slot.SetInventoryItemType(inventoryItemType, slot.ItemCount);
            if (slot.InventoryBar != null)
            {
                slot.InventoryBar.InventoryListItems[slot.InventoryBarIndex].SetInventoryItem(slot);
                
                if (slot.InventoryBar.CurrentIndex == slot.InventoryBarIndex)
                {
                    slot.InventoryBar.SelectItem(slot.InventoryBarIndex);
                }
            }

            return true;
        }

        return false;
    }

    public void RemoveItem(InventoryItemType inventoryItemType, int count)
    {
        var inventoryList = _inventoryBarListItems.ToList();
        inventoryList.AddRange(_inventoryListItems.ToList());

        int leftCount = count;

        foreach (var inventoryListItem in inventoryList)
        {
            if (!inventoryListItem.Empty)
            {
                if (inventoryItemType == inventoryListItem.GetInventoryItemType)
                {
                    if (leftCount >= inventoryListItem.ItemCount)
                    {
                        leftCount -= inventoryListItem.ItemCount;
                        inventoryListItem.SetInventoryItemType(InventoryItemType.None, 0);
                    }
                    else
                    {
                        inventoryListItem.ItemCount -= leftCount;
                        inventoryListItem.SetInventoryItemType(inventoryListItem.GetInventoryItemType, inventoryListItem.ItemCount);
                        leftCount = 0;
                    }
                    
                    if (inventoryListItem.InventoryBar != null)
                    {
                        inventoryListItem.InventoryBar.InventoryListItems[inventoryListItem.InventoryBarIndex].SetInventoryItem(inventoryListItem);
                        
                        if (inventoryListItem.InventoryBar.CurrentIndex == inventoryListItem.InventoryBarIndex)
                        {
                            inventoryListItem.InventoryBar.SelectItem(inventoryListItem.InventoryBarIndex);
                        }
                    }
                }
            }

            if (leftCount == 0)
            {
                return;
            }
        }
    }
}
