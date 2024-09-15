using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftRecipeListItem : MonoBehaviour
{
    [SerializeField] private InventoryListItem _icon;
    [SerializeField] private TMP_Text _nameText; 
    [SerializeField] private Image _image; 
    
    [SerializeField] private Color _yesColor;
    [SerializeField] private Color _noColor;
    
    private CraftRecipe _craftRecipe;
    private InventoryUI _inventoryUI;
    private Action<CraftRecipe> _onClickAction;

    public CraftRecipe GetRecipe => _craftRecipe;

    public void SetInventoryUI(InventoryUI inventoryUI, CraftRecipe craftRecipe, Action<CraftRecipe> onClickAction)
    {
        _inventoryUI = inventoryUI;
        _craftRecipe = craftRecipe;
        _onClickAction = onClickAction;
        
        _icon.SetInventoryItemType(_craftRecipe.ResultItem, 1);
        _nameText.text = _craftRecipe.Name;
    }

    public void SetCraftAble(bool craftAble)
    {
        _image.color = craftAble ? _yesColor : _noColor;
    }

    public void OnClick()
    {
        _onClickAction?.Invoke(_craftRecipe);
    }
}
