using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class InteractItem : NetworkBehaviour
{
    [SerializeField] private List<Outline> _outlines;
    [SerializeField] private InventoryItemType _type;
    [SerializeField] private int _getCount = 1;
    
    public void Look(bool look)
    {
        _outlines.ForEach(x => x.enabled = look);
    }

    public (InventoryItemType, int) GetItem()
    {
        return (_type, _getCount);
    }
}
