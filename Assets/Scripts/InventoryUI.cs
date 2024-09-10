using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    
    [SerializeField] private GameObject _inventoryGroup;
    [SerializeField] private GameObject _craftGroup;
    
    private InventoryTab _inventoryTab = InventoryTab.Inventory;
    

    public void OnClickInventoryTab()
    {
        _inventoryTab = InventoryTab.Inventory;
        
        _inventoryTabButton.color = Color.white;
        _craftTabButton.color = Color.gray;
        
        _inventoryGroup.SetActive(true);
        _craftGroup.SetActive(false);
    }
    
    public void OnClickCraftTab()
    {
        _inventoryTab = InventoryTab.Craft;
        
        _craftTabButton.color = Color.white;
        _inventoryTabButton.color = Color.gray;
        
        _craftGroup.SetActive(true);
        _inventoryGroup.SetActive(false);
    }
}
