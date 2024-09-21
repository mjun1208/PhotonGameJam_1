using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InventoryUI
{
    [SerializeField] private GameObject _shopGroup;
    [SerializeField] private GameObject _invenGroup;
    [SerializeField] private ShopInfo _shopInfo;
    [SerializeField] private Transform _shopListParent;
    [SerializeField] private ShopListItem _originShopListItem;
    
    private List<ShopListItem> _shopListItems = new List<ShopListItem>();

    private SellItem _sellItem = null;
    private bool _init = false;
    
    public void OnClickShop()
    {
        _inventoryTab = InventoryTab.Shop;

        _shopGroup.SetActive(true);
        _invenGroup.SetActive(false);

        if (!_init)
        {
            SetShopItems(ShopManager.SellList);
            _init = true;
        }

        UpdateShopList();

        OnClickSellListItem(ShopManager.SellList[0]);
        
        // Tutorial
        Global.Instance.IngameManager.ServerOnlyGameManager.TutorialManager.SetTutorialIndex(3);
    }
    
    private void SetShopItems(List<SellItem> sellItems)
    {
        foreach (var sellItem in sellItems)
        {
            var newShopListItem = Instantiate(_originShopListItem, _shopListParent);

            newShopListItem.SetInventoryUI(sellItem, Global.Instance.IngameManager.ServerOnlyGameManager.Wave, OnClickSellListItem);
            newShopListItem.SetBuyAble(BuyAble(sellItem), Global.Instance.IngameManager.ServerOnlyGameManager.Wave);
            newShopListItem.gameObject.SetActive(true);   
            
            _shopListItems.Add(newShopListItem);
        }
    }

    private void UpdateShopList()
    {
        _shopListItems.ForEach(x=>
        {
            if (x.gameObject.activeSelf)
            {
                x.SetBuyAble(BuyAble(x.GetSellItem), Global.Instance.IngameManager.ServerOnlyGameManager.Wave);
            }
        });
    }
    
    public bool BuyAble(SellItem sellItem)
    {
        return sellItem.Price <= Global.Instance.IngameManager.ServerOnlyGameManager.RewardCount;
    }

    public void Buy()
    {
        if (!BuyAble(_sellItem))
        {
            _player.ShowNotice("재화가 부족합니다", Color.red);
            return;
        }

        Global.Instance.IngameManager.ServerOnlyGameManager.RpcRemoveReward(_sellItem.Price);
        
        AddItem(_sellItem.InventoryItemType, _sellItem.Count);
        
        _shopInfo.SetInfo(_sellItem);
        
        _craftEnd.Show(_sellItem);
        
        UpdateShopList();
    }
    
    public void OnClickSellListItem(SellItem sellItem)
    {
        if (sellItem.Wave > Global.Instance.IngameManager.ServerOnlyGameManager.Wave)
        {
            _player.ShowNotice("라운드 수가 부족합니다.", Color.red);
            return;
        }
        
        _sellItem = sellItem;
        _shopInfo.SetInfo(sellItem);
    }
}
