using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class Table : NetworkBehaviour
{
    public Transform TableSit1;
    public Transform TableSit2;

    public bool IsTableSit1;
    public bool IsTableSit2;
    
    public GameObject SizeObject;

    [SerializeField] private GameObject _rewardObject;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private Outline _outline;
    [Networked, OnChangedRender(nameof(SetNetworkedRewardCount))] public int RewardCount { get; set; }

    public void Look(bool look)
    {
        _outline.enabled = look;
    }
    
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
        IsTableSit1 = true;
        npc.TargetTable = this;
        
        var npc2 = Runner.Spawn(Global.Instance.IngameManager.Npc, Global.Instance.IngameManager.NpcSpawnPosition.position, Quaternion.identity, Object.StateAuthority);
        npc2.TargetSit = GetEmptySit();
        IsTableSit2 = true;
        npc2.TargetTable = this;
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

    public void ReceiveReward()
    {
        SetReward(0);
    }

    public void SetReward(int count)
    {
        RewardCount = count;
        _rewardText.text = count.ToString();
        
        if (RewardCount > 0)
        {
            _rewardObject.SetActive(true);
        }
        else
        {
            _rewardObject.SetActive(false);
        }
    }
    
    public void SetNetworkedRewardCount()
    {
        if (HasStateAuthority)
        {
            return;
        }
       
        _rewardText.text = RewardCount.ToString();
        
        if (RewardCount > 0)
        {
            _rewardObject.SetActive(true);
        }
        else
        {
            _rewardObject.SetActive(false);
        }
    }
}
