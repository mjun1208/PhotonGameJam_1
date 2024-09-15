using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftInfo : MonoBehaviour
{
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private InventoryListItem _icon;
    [SerializeField] private List<MaterialListItem> _materialList;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descText;

    public void SetInfo(CraftRecipe craftRecipe)
    {
        _icon.SetInventoryItemType(craftRecipe.ResultItem, 1);
        _nameText.text = craftRecipe.Name;
        _descText.text = craftRecipe.Desc;

        int materialIndex = 0;
        _materialList.ForEach(x=> x.gameObject.SetActive(false));

        foreach (var materialInfo in craftRecipe.Material)
        {
            int itemCount = _inventoryUI.GetInventoryItemCount(materialInfo.Key);
            
            _materialList[materialIndex].SetInfo(materialInfo.Key, materialInfo.Value, itemCount);
            _materialList[materialIndex].gameObject.SetActive(true);
            materialIndex++;
        }
    }
}
