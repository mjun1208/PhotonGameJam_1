using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItem
{
    public InventoryItemType InventoryItemType;
    public int Wave;
    public int Price;
    public int Count;
    public string Name;
    public string Desc;
}

public static class ShopManager
{
    public static List<SellItem> SellList = new List<SellItem>()
    {
        new SellItem()
        {
            InventoryItemType = InventoryItemType.SeedBag_Corn,
            Wave = 0,
            Price = 300,
            Count = 5,
            Name = "옥수수 씨앗",
            Desc = "순식간에 자라버리는 옥수수 씨앗이다."
        },
        
        new SellItem()
        {
            InventoryItemType = InventoryItemType.SeedBag_Carrot,
            Wave = 3,
            Price = 500,
            Count = 5,
            Name = "당근 씨앗",
            Desc = "평범한 당근."
        },
        
        new SellItem()
        {
            InventoryItemType = InventoryItemType.SeedBag_Cola,
            Wave = 10,
            Price = 1000,
            Count = 5,
            Name = "콜라 씨앗",
            Desc = "투명한 유리병의 콜라가 자라는 신비로운 식물의 씨앗."
        },
    };
}

public class Shop : MonoBehaviour
{
    [SerializeField] private List<Outline> _outlines;
    
    public void Look(bool look)
    {
        _outlines.ForEach(x => x.enabled = look);
    }
}
