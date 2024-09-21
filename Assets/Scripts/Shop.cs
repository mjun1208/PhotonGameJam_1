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
            InventoryItemType = InventoryItemType.SeedBag_Corn,
            Wave = 3,
            Price = 300,
            Count = 5,
            Name = "옥수수 씨앗",
            Desc = "순식간에 자라버리는 옥수수 씨앗이다."
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
