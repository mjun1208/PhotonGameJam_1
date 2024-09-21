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

    // public NPC TableSit1Npc;
    // public NPC TableSit2Npc;
    
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

        Global.Instance.IngameManager.Tables.Add(this);

        if (Global.Instance.IngameManager.IsTabling)
        {
            ShowSizeObject(true);
        }
        
        Global.Instance.IngameManager.UpdateMap();
        
        if (HasStateAuthority)
        {
            Global.Instance.IngameManager.ServerOnlyGameManager.OnEmptySitAppear();
        }
        
        if (HasStateAuthority)
        {
            if (Global.Instance.IngameManager.ServerOnlyGameManager.TutorialIndex == 9)
            {
                Global.Instance.IngameManager.ServerOnlyGameManager.SpawnTutorialNpc();
            }
        }
    }

    public (Table, Transform) GetEmptySit()
    {
        if (!IsTableSit1)
        {
            IsTableSit1 = true;
            return (this, TableSit1);
        }
        
        if (!IsTableSit2)
        {
            IsTableSit2 = true;
            return (this, TableSit2);
        }

        return (null, null);
    }

    public void SetSitEmpty(Transform sit)
    {
        if (sit == TableSit1)
        {
            IsTableSit1 = false;
        }

        if (sit == TableSit2)
        {
            IsTableSit2 = false;
        }

        if (HasStateAuthority)
        {
            Global.Instance.IngameManager.ServerOnlyGameManager.OnEmptySitAppear();
        }
    }

    public void ShowSizeObject(bool show)
    {
        SizeObject.SetActive(show);
    }

    public void ReceiveReward()
    {
        Global.Instance.IngameManager.ServerOnlyGameManager.RewardCount += RewardCount;
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
