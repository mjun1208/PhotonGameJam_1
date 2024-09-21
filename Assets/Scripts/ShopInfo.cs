using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
    [SerializeField] private InventoryListItem _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descText;
    [SerializeField] private TMP_Text _priceText;

    public void SetInfo(SellItem sellItem)
    {
        _icon.SetInventoryItemType(sellItem.InventoryItemType, 1);
        _nameText.text = sellItem.Name;
        _descText.text = sellItem.Desc;
        if (Global.Instance.IngameManager.ServerOnlyGameManager.RewardCount >= sellItem.Price)
        {
            _priceText.text = $"{Global.Instance.IngameManager.ServerOnlyGameManager.RewardCount} / {sellItem.Price}";
        }
        else
        {
            _priceText.text = $"<color=#FF5146>{Global.Instance.IngameManager.ServerOnlyGameManager.RewardCount}</color> / {sellItem.Price}";
        }
    }
}
