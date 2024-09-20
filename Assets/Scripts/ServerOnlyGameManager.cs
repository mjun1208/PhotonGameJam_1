using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using TMPro;
using UnityEngine;

public class Balance
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
    
    [SerializeField] private TMP_Text _leftNpcText;
    [SerializeField] private TMP_Text _endNpcText;
    
    [Networked, OnChangedRender(nameof(OnChangedWaveCount))] public int Wave { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnChangedRewardCount))] public int RewardCount { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnChangedLeftNpcCount))] public int ThisTimeNpcCount { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnChangedEndNpcCount))] public int EndNpcCount { get; set; } = 0;
    
    private int _serverOnlyThisTimeNpcCount { get; set; } = 0;

    public TutorialManager TutorialManager;
    
    [Networked, OnChangedRender(nameof(OnChangedTutorialIndex))] public int TutorialIndex { get; set; } = 0;

    private List<Balance> _balances = new List<Balance>()
    {
        new Balance() {Wave = 1, NpcCount = 3, LimitTime = 120,}, // 옥수수죽
        new Balance() {Wave = 2, NpcCount = 5, LimitTime = 120,}, // 낚시 오픈? 생선
        new Balance() {Wave = 3, NpcCount = 8, LimitTime = 120,}, // 도넛, 커피
        new Balance() {Wave = 4, NpcCount = 11, LimitTime = 120,},
        new Balance() {Wave = 5, NpcCount = 15, LimitTime = 120,},
        
        new Balance() {Wave = 6, NpcCount = 20, LimitTime = 150,}, // 햄버거, 콜라
        new Balance() {Wave = 7, NpcCount = 22, LimitTime = 150,},
        new Balance() {Wave = 8, NpcCount = 24, LimitTime = 150,},
        new Balance() {Wave = 9, NpcCount = 26, LimitTime = 150,}, // 케이크
        new Balance() {Wave = 10, NpcCount = 28, LimitTime = 150,},
        
        new Balance() {Wave = 11, NpcCount = 30, LimitTime = 180,},
        new Balance() {Wave = 12, NpcCount = 33, LimitTime = 180,}, // 블라블라 1 와인?
        new Balance() {Wave = 13, NpcCount = 36, LimitTime = 180,},
        new Balance() {Wave = 14, NpcCount = 39, LimitTime = 180,},
        new Balance() {Wave = 15, NpcCount = 45, LimitTime = 180,}, // 블라블라 2 
        
        new Balance() {Wave = 16, NpcCount = 50, LimitTime = 200,}, // 라멘
        new Balance() {Wave = 17, NpcCount = 55, LimitTime = 200,}, 
        new Balance() {Wave = 18, NpcCount = 60, LimitTime = 200,},
        new Balance() {Wave = 19, NpcCount = 65, LimitTime = 200,},
        new Balance() {Wave = 20, NpcCount = 77, LimitTime = 200,},
    };

    private Balance GetBalance(int wave)
    {
        return _balances.FirstOrDefault(x => x.Wave == wave);
    }

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
        if (HasStateAuthority)
        {
            if (Wave != 0)
            {
                var balance = GetBalance(Wave);
                ThisTimeNpcCount = balance.NpcCount;
                _serverOnlyThisTimeNpcCount = balance.NpcCount;
                EndNpcCount = 0;
                
                WaveStart();
            }
        }

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

    public void OnChangedLeftNpcCount()
    {
        _leftNpcText.text = $"남은 손님 : {ThisTimeNpcCount}";
    }

    public void OnChangedEndNpcCount()
    {
        _endNpcText.text = $"끝 : {EndNpcCount}";

        if (HasStateAuthority)
        {
            if (EndNpcCount == _serverOnlyThisTimeNpcCount)
            {
                Wave++;
            }
        }
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
    
    public async void OnChangedTutorialIndex()
    {
        TutorialManager.OnChangedTutorialIndex(out bool isFinal);

        if (isFinal)
        {
            await HideTutorialUI();
            
            if (HasStateAuthority)
            {
                Wave = 1;
            }
        }
    }

    private async UniTask HideTutorialUI()
    {
        await UniTask.Delay(3000);
        
        TutorialManager.HideTutorialUI();
    }

    public void WaveStart()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        int npcCount = ThisTimeNpcCount;

        for (int i = 1; i <= npcCount; i++)
        {
            var emptySit = Global.Instance.IngameManager.GetTableSit();

            if (emptySit.Item1 != null && emptySit.Item2 != null)
            {
                SpawnNpc(emptySit.Item1, emptySit.Item2);
            } 
        }
    }

    public void OnEmptySitAppear()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        WaveStart();
    }

    public void SpawnNpc(Table table, Transform sit)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        var npc = Runner.Spawn(Global.Instance.IngameManager.Npc, Global.Instance.IngameManager.NpcSpawnPosition.position + new Vector3(0, 0.5f, 0), Quaternion.identity, Object.StateAuthority);
        npc.TargetSit = sit;
        npc.TargetTable = table;

        ThisTimeNpcCount--;
    }
}
