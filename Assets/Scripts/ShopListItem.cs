using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopListItem : MonoBehaviour
{
    [SerializeField] private InventoryListItem _icon;
    [SerializeField] private TMP_Text _nameText; 
    [SerializeField] private Image _image; 
    
    [SerializeField] private Color _yesColor;
    [SerializeField] private Color _noColor;
    
    [SerializeField] private GameObject _lockIcon;
    [SerializeField] private TMP_Text _lockText;

    private SellItem _sellItem;
    private Action<SellItem> _onClickAction;
    
    public SellItem GetSellItem => _sellItem;
    
    public void SetInventoryUI(SellItem sellItem, int wave, Action<SellItem> onClickAction)
    {
        _sellItem = sellItem;
        _onClickAction = onClickAction;
        
        _icon.SetInventoryItemType(sellItem.InventoryItemType, 1);
        _nameText.text = sellItem.Name;

        _lockIcon.SetActive(sellItem.Wave > wave);
        _lockText.text = $"{sellItem.Wave} 라운드 이후 해제";
    }

    public void SetBuyAble(bool buyAble, int wave)
    {
        _image.color = buyAble ? _yesColor : _noColor;
        
        _lockIcon.SetActive(_sellItem.Wave > wave);
    }

    public void OnClick()
    {
        _onClickAction?.Invoke(_sellItem);
    }
}
