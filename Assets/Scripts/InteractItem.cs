using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class InteractItem : NetworkBehaviour
{
    [SerializeField] private List<Outline> _outlines;
    [SerializeField] private InventoryItemType _type;
    [SerializeField] private int _getCount = 1;

    public bool SpawnDelay;

    private void Start()
    {
        SpawnDelay = true;
    }

    public override void Spawned()
    {
        base.Spawned();

        PlaySpawnDelay();
    }

    public async void PlaySpawnDelay()
    {
        SpawnDelay = true;
        
        await UniTask.Delay(500);
        
        SpawnDelay = false;
    }

    public void Look(bool look)
    {
        _outlines.ForEach(x => x.enabled = look);
    }

    public (InventoryItemType, int) GetItem()
    {
        return (_type, _getCount);
    }
}
