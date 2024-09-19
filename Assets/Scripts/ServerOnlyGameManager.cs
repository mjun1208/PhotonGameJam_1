using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class Balacne
{
    public int Wave;
    public int LimitTime;
    public int NpcCount;
}

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

    private List<Balacne> _balacnes = new List<Balacne>()
    {
        new Balacne() {Wave = 1, NpcCount = 3, LimitTime = 120,}, // 옥수수죽, 상어주스
        new Balacne() {Wave = 2, NpcCount = 5, LimitTime = 120,}, 
        new Balacne() {Wave = 3, NpcCount = 8, LimitTime = 120,}, // 도넛, 커피
        new Balacne() {Wave = 4, NpcCount = 11, LimitTime = 120,},
        new Balacne() {Wave = 5, NpcCount = 15, LimitTime = 120,},
        
        new Balacne() {Wave = 6, NpcCount = 20, LimitTime = 150,}, // 햄버거, 콜라
        new Balacne() {Wave = 7, NpcCount = 22, LimitTime = 150,},
        new Balacne() {Wave = 8, NpcCount = 24, LimitTime = 150,},
        new Balacne() {Wave = 9, NpcCount = 26, LimitTime = 150,}, // 케이크
        new Balacne() {Wave = 10, NpcCount = 28, LimitTime = 150,},
        
        new Balacne() {Wave = 11, NpcCount = 30, LimitTime = 180,},
        new Balacne() {Wave = 12, NpcCount = 33, LimitTime = 180,}, // 블라블라 1 와인?
        new Balacne() {Wave = 13, NpcCount = 36, LimitTime = 180,},
        new Balacne() {Wave = 14, NpcCount = 39, LimitTime = 180,},
        new Balacne() {Wave = 15, NpcCount = 45, LimitTime = 180,}, // 블라블라 2 
        
        new Balacne() {Wave = 16, NpcCount = 50, LimitTime = 200,}, // 라멘
        new Balacne() {Wave = 17, NpcCount = 55, LimitTime = 200,}, 
        new Balacne() {Wave = 18, NpcCount = 60, LimitTime = 200,},
        new Balacne() {Wave = 19, NpcCount = 65, LimitTime = 200,},
        new Balacne() {Wave = 20, NpcCount = 77, LimitTime = 200,},
    };

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            Wave++;
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
        TutorialManager.OnChangedTutorialIndex(out bool isFinal);

        if (isFinal)
        {
            HideTutorialUI();
            
            if (HasStateAuthority)
            {
                Wave = 1;
            }
        }
    }

    private async void HideTutorialUI()
    {
        await UniTask.Delay(3000);
        
        TutorialManager.HideTutorialUI();
    }
}
