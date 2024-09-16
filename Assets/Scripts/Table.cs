using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Table : NetworkBehaviour
{
    public Transform TableSit1;
    public Transform TableSit2;

    public bool IsTableSit1;
    public bool IsTableSit2;
    
    public GameObject SizeObject;

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            RpcSpawnNpc();
        }
        
        Global.Instance.IngameManager.Tables.Add(this);

        if (Global.Instance.IngameManager.IsTabling)
        {
            ShowSizeObject(true);
        }
        
        Global.Instance.IngameManager.UpdateMap();
    }
    
    public void RpcSpawnNpc()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        var npc = Runner.Spawn(Global.Instance.IngameManager.Npc, Global.Instance.IngameManager.NpcSpawnPosition.position, Quaternion.identity, Object.StateAuthority);
        npc.TargetSit = GetEmptySit();
    }

    public Transform GetEmptySit()
    {
        if (!IsTableSit1)
        {
            return TableSit1;
        }
        
        if (!IsTableSit2)
        {
            return TableSit2;
        }

        return null;
    }

    // public void Update()
    // {
    //     throw new NotImplementedException();
    // }

    public void ShowSizeObject(bool show)
    {
        SizeObject.SetActive(show);
    }
}
