using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using TMPro;
using UnityEngine;

public class ServerOnlyGameManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private DOTweenAnimation _waveTextDOTween;
    [SerializeField] private DOTweenAnimation _rewardTextDOTween;
    
    [Networked, OnChangedRender(nameof(OnChangedWaveCount))] public int Wave { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnChangedRewardCount))] public int RewardCount { get; set; } = 0;
    
    public int ThisTimeNpcCount { get; set; } = 1;

    public TutorialManager TutorialManager;
    
    [Networked, OnChangedRender(nameof(OnChangedTutorialIndex))] public int TutorialIndex { get; set; } = 0;

    public override void Spawned()
    {
        base.Spawned();
        
        Global.Instance.IngameManager.ServerOnlyGameManager = this;

        if (HasStateAuthority)
        {
            Wave = 0;
            RewardCount = 0;
        }
        
        OnChangedWaveCount();
        OnChangedRewardCount();
    }

    public void OnChangedWaveCount()
    {
        _waveText.text = $"{Wave} 라운드";
        _waveTextDOTween.DORewind();
        _waveTextDOTween.DOPlay();
    }
    
    public void OnChangedRewardCount()
    {
        _rewardText.text = RewardCount.ToString();
        _rewardTextDOTween.DORewind();
        _rewardTextDOTween.DOPlay();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RpcSetTutorialIndex(TutorialIndex + 1);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetTutorialIndex(int index)
    {
        if (HasStateAuthority)
        {
            if (TutorialIndex < index)
            {
                TutorialIndex = index;
            }
        }
    }
    
    public void OnChangedTutorialIndex()
    {
        TutorialManager.OnChangedTutorialIndex();
    }
}
