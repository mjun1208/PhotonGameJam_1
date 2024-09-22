using System.Collections.Generic;
using UnityEngine;

public class InventoryBar : MonoBehaviour
{
    [SerializeField] private List<InventoryListItem> _inventoryListItems;
    [SerializeField] private InventoryUI _inventoryUI;

    private int _selectedIndex = 0;
    private InventoryListItem _currentSelectedItem;
    
    public List<InventoryListItem> InventoryListItems => _inventoryListItems;
    public int CurrentIndex => _selectedIndex;// 

    public void SetPlayer(Player player)
    {
        _inventoryListItems.ForEach(x=> x.SetPlayer(player));
        
        // TestCow
        _inventoryListItems[0].SetInventoryItemType(InventoryItemType.Axe, 1);
        _inventoryListItems[1].SetInventoryItemType(InventoryItemType.SeedBag_Corn, 5);
        // _inventoryListItems[2].SetInventoryItemType(InventoryItemType.Table, 300);
        // _inventoryListItems[3].SetInventoryItemType(InventoryItemType.CornSoup, 300);
        // _inventoryListItems[4].SetInventoryItemType(InventoryItemType.FishRod, 1);
        //
        // _inventoryListItems[5].SetInventoryItemType(InventoryItemType.Axe_1, 1);
        // _inventoryListItems[6].SetInventoryItemType(InventoryItemType.Axe_2, 1);
        // _inventoryListItems.ForEach(x=> x.SetInventoryItemType((InventoryItemType)_inventoryListItems.IndexOf(x), 1));

        SelectItem(0);
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
