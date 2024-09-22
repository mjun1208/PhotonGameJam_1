using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ChestUI : InventoryUI
{
    [SerializeField] protected List<InventoryListItem> _chestListItems;
    [SerializeField] InventoryUI _inventoryUI;
    
    public Chest Chest { get; set; }

    private void SetChest(Chest chest)
    {
        Chest = chest;
    }

    public void SetUpChestInventory(Chest chest)
    {
        SetChest(chest);

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

        for (int i = 0; i < _inventoryListItems.Count; i++)
        {
            _inventoryListItems[i].SetInventoryUI(this);
            _inventoryListItems[i].SetInventoryItemType(_inventoryUI._inventoryListItems[i].GetInventoryItemType, _inventoryUI._inventoryListItems[i].ItemCount);
            
            if (_inventoryUI._inventoryListItems[i].Empty)
            {
                _inventoryListItems[i].SetEmpty();
            }
        }

        for (int i = 0; i < _chestListItems.Count; i++)
        {
            _chestListItems[i].SetInventoryUI(this);
            _chestListItems[i].SetInventoryItemType((InventoryItemType)Chest.ChestInventoryItems[i].t, Chest.ChestInventoryItems[i].c);
            
            if (Chest.ChestInventoryItems[i].e)
            {
                _chestListItems[i].SetEmpty();
            }
        }
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();

        for (int i = 0; i < _inventoryUI._inventoryListItems.Count; i++)
        {
            // _inventoryUI._inventoryListItems[i].SetInventoryUI(this);
            _inventoryUI._inventoryListItems[i].SetInventoryItemType(_inventoryListItems[i].GetInventoryItemType, _inventoryListItems[i].ItemCount);
            
            if (_inventoryUI._inventoryListItems[i].Empty)
            {
                _inventoryUI._inventoryListItems[i].SetEmpty();
            }
        }

        for (int i = 0; i < _inventoryUI._inventoryBarListItems.Count; i++)
        {
            // _inventoryUI._inventoryBarListItems[i].SetInventoryUI(this);
            _inventoryUI._inventoryBarListItems[i].SetInventoryItemType(_inventoryBarListItems[i].GetInventoryItemType, _inventoryBarListItems[i].ItemCount);
            
            if (_inventoryUI._inventoryBarListItems[i].Empty)
            {
                _inventoryUI._inventoryBarListItems[i].SetEmpty();
            }
        }

        
        var saveChestList = _chestListItems.Select(x => x.ToChestInventoryItem());
        
        var networkedList = saveChestList;
        string jsonString = JsonConvert.SerializeObject(networkedList);
        
        Chest.RpcSaveChest(jsonString);
        Chest.RpcSetOpenUI(false, "");
    }
}
