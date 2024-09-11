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

public class InventoryListItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private GameObject _rayCastImage;
    [SerializeField] private GameObject _selectImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private List<Sprite> _itemIcons;
    private Player _player;
    private InventoryItemType _inventoryItemType;
    
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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Dragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Dragging = true;
    }
}
