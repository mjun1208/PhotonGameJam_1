using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct NpcWantItem_Networked
{
    // Recipe
    public int R;
    // Success
    public bool S;
    // Fail
    public bool F;
}

public class NpcWantItem : MonoBehaviour
{
    [SerializeField] private InventoryListItem _icon;
    [SerializeField] private GameObject _successImage;
    [SerializeField] private GameObject _failImage;
    [SerializeField] private TMP_Text _nameText;

    public CraftRecipe WantCraftRecipe;
    public bool IsSuccess = false;
    public bool IsFail = false;

    public void SetInfo(CraftRecipe craftRecipe)
    {
        WantCraftRecipe = craftRecipe;
        _icon.SetInventoryItemType(craftRecipe.ResultItem, 1);
        _nameText.text = WantCraftRecipe.Name;

        IsSuccess = false;
        IsFail = false;
        
        _successImage.SetActive(false);
        _failImage.SetActive(false);
    }

    public void SetSuccess()
    {
        IsSuccess = true;
        _successImage.SetActive(true);
    }
    
    public void SetFail()
    {
        IsFail = true;
        _failImage.SetActive(true);
    }

    public NpcWantItem_Networked ToNetworked()
    {
        return new NpcWantItem_Networked
        {
            R = (int)this.WantCraftRecipe.ResultItem,
            S = this.IsSuccess,
            F = this.IsFail,
        };
    }
}
