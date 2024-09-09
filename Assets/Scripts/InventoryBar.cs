using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBar : MonoBehaviour
{
    [SerializeField] private List<InventoryListItem> _inventoryListItems;

    private int _selectedIndex = 0;
    private InventoryListItem _currentSelectedItem;

    public void SetPlayer(Player player)
    {
        _inventoryListItems.ForEach(x=> x.SetPlayer(player));
        
        // Test
        _inventoryListItems.ForEach(x=> x.SetInventoryItemType((InventoryItemType)_inventoryListItems.IndexOf(x)));
    }

    public void SelectItem(int index)
    {
        _selectedIndex = index;
        
        if (_currentSelectedItem != null)
        {
            _currentSelectedItem.Select(false);
        }

        _currentSelectedItem = _inventoryListItems[index];
        _currentSelectedItem.Select(true);
    }

    public void SelectPrev()
    {
        _selectedIndex--;

        if (_selectedIndex < 0)
        {
            _selectedIndex = _inventoryListItems.Count - 1;
        }

        SelectItem(_selectedIndex);
    }
    
    public void SelectNext()
    {
        _selectedIndex++;

        if (_selectedIndex > _inventoryListItems.Count - 1)
        {
            _selectedIndex = 0;
        }
        
        SelectItem(_selectedIndex);
    }
}
