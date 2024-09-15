using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class InventoryUI : MonoBehaviour
{
    [SerializeField] private CraftInfo _craftInfo;
    [SerializeField] private Player _player;
    [SerializeField] private CraftEnd _craftEnd;

    public List<CraftRecipe> CraftRecipes = new List<CraftRecipe>()
    {
        new CraftRecipe()
        {
            ResultItem = InventoryItemType.BonFire,
            Name = "모닥불",
            Desc = "뜨끈뜨끈 하다.\n여기서 요리를 할 수 있을 것 같다",
            Material = new Dictionary<InventoryItemType, int>()
            {
                {InventoryItemType.Log, 5},
                {InventoryItemType.Axe, 1}
            }
        }
    };

    private CraftRecipe _craftRecipe = null;

    public void OnClickCraftTab()
    {
        _inventoryTab = InventoryTab.Craft;
        
        _craftTabButton.color = Color.white;
        _inventoryTabButton.color = Color.gray;
        
        _craftGroup.SetActive(true);
        _inventoryGroup.SetActive(false);

        OnClickRecipe(CraftRecipes[0]);
    }

    public int GetInventoryItemCount(InventoryItemType type)
    {
        var inventoryList = _inventoryBarListItems.ToList();
        inventoryList.AddRange(_inventoryListItems.ToList());

        int count = 0;
        
        foreach (var inventoryListItem in inventoryList)
        {
            if (!inventoryListItem.Empty)
            {
                if (type == inventoryListItem.GetInventoryItemType)
                {
                    count += inventoryListItem.ItemCount;
                }
            }
        }

        return count;
    }

    public bool CraftAble(CraftRecipe craftRecipe)
    {
        var inventoryList = _inventoryBarListItems.ToList();
        inventoryList.AddRange(_inventoryListItems.ToList());

        foreach(var material in craftRecipe.Material)
        {
            if (GetInventoryItemCount(material.Key) < material.Value)
            {
                return false;
            }
        }
        
        return true;
    }

    public void Craft()
    {
        if (!CraftAble(_craftRecipe))
        {
            _player.ShowNotice("재료가 부족합니다", Color.red);
            return;
        }
        
        foreach (var materialItem in _craftRecipe.Material)
        {
            RemoveItem(materialItem.Key, materialItem.Value);
        }
        
        AddItem(_craftRecipe.ResultItem, 1);
        
        _craftInfo.SetInfo(_craftRecipe);
        
        _craftEnd.Show(_craftRecipe);
        
        // _player.ShowNotice("제작 완료", Color.green);
    }

    private void OnDisable()
    {
        _craftEnd.gameObject.SetActive(false);
    }

    public void OnClickRecipe(CraftRecipe craftRecipe)
    {
        _craftRecipe = craftRecipe;
        _craftInfo.SetInfo(craftRecipe);
    }
}

public class CraftRecipe
{
    public InventoryItemType ResultItem;
    public string Name;
    public string Desc;
    public Dictionary<InventoryItemType, int> Material;

    // public bool CraftAble(InventoryUI inventoryUI)
    // {
    //     inventoryUI.GetFirstItemSlot()
    //     
    //     return false;
    // }
}