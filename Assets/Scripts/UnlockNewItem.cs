using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnlockNewItem : MonoBehaviour
{
    [SerializeField] private InventoryListItem _item;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _typeText;
    
    public void SetInfo(CraftRecipe recipe, bool isCook)
    {
        _item.SetInventoryItemType(recipe.ResultItem, 1);
        _nameText.text = recipe.Name;

        if (isCook)
        {
            _typeText.text = "요리";
        }
        else
        {
            _typeText.text = "제작";
        }
    }
}
