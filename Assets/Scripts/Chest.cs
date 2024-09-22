using System;
using System.Collections.Generic;
using Fusion;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

[Serializable]
public struct ChestInventoryItem
{
    // IsEmpty
    public bool e;
    
    // ItemType
    public int t;
    
    // Count
    public int c;
}

public class Chest : NetworkBehaviour
{
    [SerializeField] private Outline _outline;
    [SerializeField] private TMP_Text _userText;
    [SerializeField] private GameObject _userTextObject;

    public List<ChestInventoryItem> ChestInventoryItems { get; set; } = new List<ChestInventoryItem>()
    {
        new ChestInventoryItem() { e = true, t = -1},
        new ChestInventoryItem() { e = true, t = -1},
        new ChestInventoryItem() { e = true, t = -1},
        new ChestInventoryItem() { e = true, t = -1},
        new ChestInventoryItem() { e = true, t = -1},
        new ChestInventoryItem() { e = true, t = -1},
        new ChestInventoryItem() { e = true, t = -1},
        new ChestInventoryItem() { e = true, t = -1},
        new ChestInventoryItem() { e = true, t = -1},
    };
    
    [Networked, OnChangedRender(nameof(SetChestItemList_Networked)), Capacity(500)] private string _chestItemList_Networked { get; set; } = "";

    [Networked, OnChangedRender(nameof(OnChangedOpen))] public NetworkBool IsOpened { get; set; } = false;
    
    [Networked, OnChangedRender(nameof(OnChangedUser))] public string UserName { get; set; } = "";

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetOpenUI(bool isOpened, string name)
    {
        IsOpened = isOpened;
        UserName = name;
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSaveChest(string networkedList)
    {
        _chestItemList_Networked = networkedList;
    }

    // ServerOnly
    public void SendNetworkChestInventoryItems()
    {
        var networkedList = ChestInventoryItems;
        string jsonString = JsonConvert.SerializeObject(networkedList);
        _chestItemList_Networked = jsonString;
    }
    
    public void SetChestItemList_Networked()
    {
        if (string.IsNullOrWhiteSpace(_chestItemList_Networked))
        {
            return;
        }
      
        List<ChestInventoryItem> deserializedList = JsonConvert.DeserializeObject<List<ChestInventoryItem>>(_chestItemList_Networked);

        for (int i = 0; i < ChestInventoryItems.Count; i++)
        {
            ChestInventoryItems[i] = deserializedList[i];
        }
    }

    public void Look(bool look)
    {
        _outline.enabled = look;
    }

    private void OnChangedOpen()
    {
        _userTextObject.gameObject.SetActive(IsOpened);
    }

    private void OnChangedUser()
    {
        _userText.text = $"{UserName}이(가) 사용중";
    }
}
