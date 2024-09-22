using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Balance
{
    public int Wave;
    public int LimitTime;
    public int NpcCount;
    public int LimitFailCount;
    public int MaxNpcRequestItemCount;
}

public partial class ServerOnlyGameManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private DOTweenAnimation _waveTextDOTween;
    [SerializeField] private DOTweenAnimation _rewardTextDOTween;
    
    [SerializeField] private TMP_Text _leftNpcText;
    [SerializeField] private TMP_Text _endNpcText;
    [SerializeField] private DOTweenAnimation _leftNpcTextDOTween;
    [SerializeField] private DOTweenAnimation _endNpcTextTextDOTween;
    
    [SerializeField] private RectTransform c;
    
    [SerializeField] private CinemachineVirtualCamera _endCamera;
    [SerializeField] private GameObject _endCanvas;
    [SerializeField] private GameObject _failText;
    [SerializeField] private GameObject _successText;
    
    [Networked, OnChangedRender(nameof(OnChangedWaveCount))] public int Wave { get; set; } = 0;
    [Networked] public int CraftWave { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnChangedRewardCount))] public int RewardCount { get; set; } = 0;
    [Networked] public int ThisTimeNpcCount { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnChangedLeftNpcCount))] public int WaveNpcCount { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnChangedEndNpcCount))] public int EndNpcCount { get; set; } = 0;
    [Networked, OnChangedRender(nameof(OnChangedFailCount))] public int WantFailCount { get; set; } = 1;
    [Networked, OnChangedRender(nameof(OnChangedFailCount))] public int NpcFailCount { get; set; } = 0;

    public int MaxRequestCount = 1;
    
    public DateTime StartTime = DateTime.Now;
    
    private int _serverOnlyThisTimeNpcCount { get; set; } = 0;

    public TutorialManager TutorialManager;
    
    [Networked, OnChangedRender(nameof(OnChangedTutorialIndex))] public int TutorialIndex { get; set; } = 0;

    private List<Balance> _balances = new List<Balance>()
    {
        new Balance() {Wave = 1, NpcCount = 3, LimitTime = 120, LimitFailCount = 3, MaxNpcRequestItemCount = 2,}, // 구운 옥수수
        new Balance() {Wave = 2, NpcCount = 5, LimitTime = 120, LimitFailCount = 3, MaxNpcRequestItemCount = 2,}, // 구운 치킨
        new Balance() {Wave = 3, NpcCount = 8, LimitTime = 120, LimitFailCount = 3, MaxNpcRequestItemCount = 2,}, // 낚시, 컵, 상어
        new Balance() {Wave = 4, NpcCount = 11, LimitTime = 120, LimitFailCount = 3, MaxNpcRequestItemCount = 2,}, // (당근, 사과) 주스
        new Balance() {Wave = 5, NpcCount = 15, LimitTime = 120, LimitFailCount = 3, MaxNpcRequestItemCount = 2,}, // 좋은 도끼, 그릇, 옥수수 파이 (옥수수 +)
        
        new Balance() {Wave = 6, NpcCount = 20, LimitTime = 150, LimitFailCount = 3, MaxNpcRequestItemCount = 3,}, // 돼지
        new Balance() {Wave = 7, NpcCount = 22, LimitTime = 150, LimitFailCount = 3, MaxNpcRequestItemCount = 3,}, // (거북이 탕)
        new Balance() {Wave = 8, NpcCount = 24, LimitTime = 150, LimitFailCount = 3, MaxNpcRequestItemCount = 3,}, // 옥수수 케이크 / 옥수수 + 
        new Balance() {Wave = 9, NpcCount = 26, LimitTime = 150, LimitFailCount = 3, MaxNpcRequestItemCount = 3,}, // 소로 하는거
        new Balance() {Wave = 10, NpcCount = 28, LimitTime = 150, LimitFailCount = 3, MaxNpcRequestItemCount = 3,}, // 더 좋은 도끼 / 당근 + 사과 + 옥수수 + 나무
        
        new Balance() {Wave = 11, NpcCount = 30, LimitTime = 180, LimitFailCount = 3, MaxNpcRequestItemCount = 4,}, // 
        new Balance() {Wave = 12, NpcCount = 33, LimitTime = 180, LimitFailCount = 3, MaxNpcRequestItemCount = 4,}, // 햄버거 세트 (돼지 + 빵 + 콜라) // 콜라 씨앗
        new Balance() {Wave = 13, NpcCount = 36, LimitTime = 180, LimitFailCount = 3, MaxNpcRequestItemCount = 4,}, // 
        new Balance() {Wave = 14, NpcCount = 39, LimitTime = 180, LimitFailCount = 3, MaxNpcRequestItemCount = 4,}, // 옥수수 라멘 (3 옥수수 + 닭 + )
        new Balance() {Wave = 15, NpcCount = 45, LimitTime = 180, LimitFailCount = 3, MaxNpcRequestItemCount = 4,}, // 
        
        // new Balance() {Wave = 16, NpcCount = 50, LimitTime = 200, LimitFailCount = 3, MaxNpcRequestItemCount = 5,}, // 라멘
        // new Balance() {Wave = 17, NpcCount = 55, LimitTime = 200, LimitFailCount = 3, MaxNpcRequestItemCount = 5,}, 
        // new Balance() {Wave = 18, NpcCount = 60, LimitTime = 200, LimitFailCount = 3, MaxNpcRequestItemCount = 5,},
        // new Balance() {Wave = 19, NpcCount = 65, LimitTime = 200, LimitFailCount = 3, MaxNpcRequestItemCount = 5,},
        // new Balance() {Wave = 20, NpcCount = 77, LimitTime = 200, LimitFailCount = 3, MaxNpcRequestItemCount = 5,},
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
            TutorialIndex = 0;
            ThisTimeNpcCount = 0;
            NpcFailCount = 0;
            WantFailCount = 1;
            StartTime = DateTime.Now;

            // 시작 시 오브젝트들을 소환
            StartCoroutine(InitialSpawn());
        }
        
        OnChangedWaveCount();
        OnChangedRewardCount();
        OnChangedTutorialIndex();
        OnChangedFailCount();
        OnChangedLeftNpcCount();
    }

    public void OnChangedWaveCount()
    {
        if (HasStateAuthority)
        {
            if (Wave != 0)
            {
                if (Wave > _balances.Count)
                {
                    RpcEndCall(true);
                    return;
                }
                
                var balance = GetBalance(Wave);
                ThisTimeNpcCount = balance.NpcCount;
                WaveNpcCount = balance.NpcCount;
                _serverOnlyThisTimeNpcCount = balance.NpcCount;
                EndNpcCount = 0;
                NpcFailCount = 0;
                WantFailCount = balance.LimitFailCount;
                MaxRequestCount = balance.MaxNpcRequestItemCount;
                WaveStart();
            }
            
            CraftWave = Wave;
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
        _leftNpcText.text = $"남은 손님 : {WaveNpcCount - EndNpcCount}";
        _leftNpcTextDOTween.DORewind();
        _leftNpcTextDOTween.DOPlay();
    }

    public void OnChangedEndNpcCount()
    {
        OnChangedLeftNpcCount();
        // _endNpcText.text = $"끝 : {EndNpcCount}";

        if (HasStateAuthority)
        {
            if (EndNpcCount == _serverOnlyThisTimeNpcCount)
            {
                if (Wave == 0)
                {
                    Wave = 1;
                }
                else
                {
                    NextWave();
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcOpenUnlockUI(int wave)
    {
        Global.Instance.MyPlayer.OpenUnlockUI(wave);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private async void RpcNextWave()
    {
        if (HasStateAuthority)
        {
            return;
        }
        
        c.gameObject.SetActive(true);

        c.anchoredPosition = new Vector2(0, 56);
        c.DOAnchorPos(new Vector2(0, -18), 0.3f);
        
        await UniTask.Delay(10000);

        await c.DOAnchorPos(new Vector2(0, 56), 0.3f);
        
        c.gameObject.SetActive(false);
    }

    private async void NextWave()
    {
        CraftWave += 1;
        RpcOpenUnlockUI(CraftWave);

        RpcNextWave();
        
        c.gameObject.SetActive(true);

        c.anchoredPosition = new Vector2(0, 56);
        c.DOAnchorPos(new Vector2(0, -18), 0.3f);
        
        await UniTask.Delay(10000);

        await c.DOAnchorPos(new Vector2(0, 56), 0.3f);
        
        c.gameObject.SetActive(false);
        
        Wave++;
    }

    public void HideCow()
    {
        c.gameObject.SetActive(false);
    }

    public void OnChangedFailCount()
    {
        _endNpcText.text = $"실패 : {NpcFailCount} / {WantFailCount}";
        _endNpcTextTextDOTween.DORewind();
        _endNpcTextTextDOTween.DOPlay();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSetCameraToFailMan(NPC lastFailNpc)
    {
        _endCamera.m_Follow = lastFailNpc.transform;
        _endCamera.m_LookAt = lastFailNpc.transform;

        Time.timeScale = 0.5f;
        
        CinemachineBrain.SoloCamera = _endCamera;
        
        End(false);
    }

    private void MakeResult(bool success)
    {
        var gameResult = new GameResult()
        {
            Wave = Math.Clamp(Wave, 0, _balances.Count),
            Success = success,
            Gold = RewardCount,
            StartTime = StartTime,
            // WhoNpcGive
            // WhoWalk
            // WhoJump
            // WhoTreeCut
            // WhoCraft
            // WhoCook
            // WhoFishing
        };
        
        Global.Instance.LastGameResult = gameResult;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcEndCall(bool isSuccess)
    {
        End(isSuccess);
    }
    
    private async void End(bool isSuccess)
    {
        MakeResult(isSuccess);
        
        await UniTask.Delay(1000);

        Time.timeScale = 1f;
        
        _endCanvas.gameObject.SetActive(true);

        _failText.SetActive(!isSuccess);
        _successText.SetActive(isSuccess);
        
        await UniTask.Delay(2000);

        SceneManager.LoadScene("EndScene");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetTutorialIndex(int index)
    {
        if (HasStateAuthority)
        {
            // 직전 Index만 적용
            if (TutorialIndex == index - 1)
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

    public async void WaveStart()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        while (ThisTimeNpcCount > 0)
        {
            if (ThisTimeNpcCount > 0)
            {
                var emptySit = Global.Instance.IngameManager.GetTableSit();

                if (emptySit.Item1 != null && emptySit.Item2 != null)
                {
                    SpawnNpc(emptySit.Item1, emptySit.Item2);
                }
            }
            
            if (ThisTimeNpcCount > 0)
            {
                var emptySit = Global.Instance.IngameManager.GetTableSit();

                if (emptySit.Item1 != null && emptySit.Item2 != null)
                {
                    SpawnNpc(emptySit.Item1, emptySit.Item2);
                }
            }
            
            if (ThisTimeNpcCount > 0)
            {
                var emptySit = Global.Instance.IngameManager.GetTableSit();

                if (emptySit.Item1 != null && emptySit.Item2 != null)
                {
                    SpawnNpc(emptySit.Item1, emptySit.Item2);
                }
            }

            await UniTask.Delay(2000);
        }

        // int npcCount = ThisTimeNpcCount;
        //
        // for (int i = 1; i <= npcCount; i++)
        // {
        //     var emptySit = Global.Instance.IngameManager.GetTableSit();
        //
        //     if (emptySit.Item1 != null && emptySit.Item2 != null)
        //     {
        //         SpawnNpc(emptySit.Item1, emptySit.Item2);
        //     } 
        // }
    }

    public void OnEmptySitAppear()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        // WaveStart();
    }

    private bool _isSpawnedTutorialNpc = false;
    
    public void SpawnTutorialNpc()
    {
        if (_isSpawnedTutorialNpc)
        {
            return;
        }

        _isSpawnedTutorialNpc = true;
            
        // 튜토리얼용 Npc 소환!
        var emptySit = Global.Instance.IngameManager.GetTableSit();

        if (emptySit.Item1 != null && emptySit.Item2 != null)
        {
            Global.Instance.IngameManager.ServerOnlyGameManager.SpawnNpc(emptySit.Item1, emptySit.Item2);
        }
    }
    
    public void SpawnNpc(Table table, Transform sit)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        var aaaNpc = Global.Instance.IngameManager.Npc;
        
        if (Random.Range(0, 2) == 1)
        {
            aaaNpc = Global.Instance.IngameManager.Npc2;
        }
        
        var npc = Runner.Spawn(aaaNpc, Global.Instance.IngameManager.NpcSpawnPosition.position + new Vector3(0, 0.5f, 0), Quaternion.identity, Object.StateAuthority);
        npc.TargetSit = sit;
        npc.TargetTable = table;

        if (Wave == 0)
        {
            npc.IsTutorial = true;
        }

        ThisTimeNpcCount--;
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcRemoveReward(int count)
    {
        RewardCount -= count;
    }
}
