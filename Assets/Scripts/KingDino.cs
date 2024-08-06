using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using Photon.Voice.Fusion.Demo;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class KingDino : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _fireFx;
    [SerializeField] private ParticleSystem _stampFx;
    [SerializeField] private ParticleSystem _spawnWavePrefabFx;
    [SerializeField] private ParticleSystem _stoneFx;
    
    [SerializeField] private GameObject _stampColl;
    [SerializeField] private CallColl _callColl;

    private List<CallColl> _callCollList = new List<CallColl>();
    
    [Networked] private NetworkBool _isMove { get; set; }
    [Networked] private int _targetPlayerId { get; set; } = 0;
    private Player _targetPlayer { get; set; }

    private PrefabSpawner _prefabSpawner = null;
    [Networked] private float _angle { get; set; } = 0;
    [Networked] private DinoState DinoStateHAHAHA { get; set; }
    private float _stateCoolTime = 0;

    enum DinoState
    {
        Walk,
        Run,
        Fire,
        Stamp,
        Stamp2,
        Call,
    }

    public void SetPrefabSpawner(PrefabSpawner prefabSpawner)
    {
        _prefabSpawner = prefabSpawner;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (HasStateAuthority)
            {
                GetTarget();
            }
        }
    }

    private void ChangeState()
    {
        if (_stateCoolTime <= 0)
        {
            if (DinoStateHAHAHA == DinoState.Run)
            {
                int enumCount = Enum.GetValues(typeof(DinoState)).Length;
                DinoStateHAHAHA = (DinoState) Random.Range(2, enumCount);
            }
            else
            {
                int enumCount = Enum.GetValues(typeof(DinoState)).Length;
                DinoStateHAHAHA = (DinoState) Random.Range(1, enumCount);
            }

            switch (DinoStateHAHAHA)
            {
                case DinoState.Fire:
                {
                    RpcFireAnime();
                    _stateCoolTime = 99;
                    break;
                }
                case DinoState.Stamp:
                {
                    RpcStampAnime();
                    _stateCoolTime = 99;
                    break;
                }
                case DinoState.Stamp2:
                {
                    RpcStamp2Anime();
                    _stateCoolTime = 99;
                    break;  
                }
                case DinoState.Call:
                {
                    RpcCallAnime();
                    _stateCoolTime = 99;
                    break;  
                }
                default:
                {
                    _stateCoolTime = Random.Range(3f, 5f);
                    break;
                }
            }
        }
        else
        {
            _stateCoolTime -= Time.deltaTime;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            switch (DinoStateHAHAHA)
            {
                case DinoState.Walk:
                {
                    Move(false);
                    
                    ChangeState();
                    break;
                }   
                case DinoState.Run:
                {
                    Move(true);
                    
                    ChangeState();
                    break;
                }
                case DinoState.Fire:
                {
                    FireMove();
                    break;
                }
                case DinoState.Stamp:
                {
                    break;
                }
                case DinoState.Stamp2:
                {
                    break;
                }
                case DinoState.Call:
                {
                    break;
                }
            }
            
            if (DinoStateHAHAHA != DinoState.Walk)
            {
                _animator.SetBool("Move", false);
            }

            if (DinoStateHAHAHA != DinoState.Run)
            {
                _animator.SetBool("Run", false);
            }
        }
        else
        {
        }
    }

    public override void Render()
    {
        if (!HasStateAuthority)
        {
            switch (DinoStateHAHAHA)
            {
                case DinoState.Walk:
                {
                    MoveRender(false);
                    break;
                }   
                case DinoState.Run:
                {
                    MoveRender(true);
                    break;
                }
                case DinoState.Fire:
                {
                    FireMove();
                    break;
                }
                case DinoState.Stamp:
                {
                    break;
                }
                case DinoState.Stamp2:
                {
                    break;
                }
                case DinoState.Call:
                {
                    break;
                }
            }

            if (DinoStateHAHAHA != DinoState.Walk)
            {
                _animator.SetBool("Move", false);
            }

            if (DinoStateHAHAHA != DinoState.Run)
            {
                _animator.SetBool("Run", false);
            }
        }
    }
    
    private void GetTarget()
    {
        if (_prefabSpawner != null)
        {
            int playerCount = _prefabSpawner.spawnedPlayers.Count;
            int randomPick = Random.Range(0, playerCount);

            var playerObject = _prefabSpawner.spawnedPlayers.ToList()[randomPick].Value;

            var playerScript = playerObject.GetComponent<Player>();
            _targetPlayer = playerScript;
            _targetPlayerId = _targetPlayer.MyPlayerRef.PlayerId;
        }
    }
    
    private void Move(bool run)
    {
        if (_targetPlayerId != 0)
        {
            var dir = _targetPlayer.transform.position - this.transform.position;
            dir.Normalize();

            var move = new Vector3(dir.x, 0, dir.z) * Time.fixedDeltaTime;

            if (run)
            {
                move *= 5f;
            }
            else
            {
                move *= 3f;
            }
            
            this.transform.position += move;
            
            var angle = Mathf.Atan2(-dir.z, dir.x) * Mathf.Rad2Deg;
            _angle = Mathf.LerpAngle(_angle, angle, 0.1f);
            this.transform.rotation = Quaternion.Euler(new Vector3(0, _angle - 90, 0));

            if (run)
            {
                _animator.SetBool("Run", true);
            }
            else
            {
                _animator.SetBool("Move", true);   
            }
        }
    }

    private void MoveRender(bool run)
    {
        if (_targetPlayerId != 0)
        {
            if (run)
            {
                _animator.SetBool("Run", true);
            }
            else
            {
                _animator.SetBool("Move", true);   
            }
            
            this.transform.rotation = Quaternion.Euler(new Vector3(0, _angle - 90, 0));
        }
    }

    private void FireMove()
    {
        if (HasStateAuthority)
        {
            if (_targetPlayerId != 0)
            {
                var dir = _targetPlayer.transform.position - this.transform.position;
                dir.Normalize();

                var angle = Mathf.Atan2(-dir.z, dir.x) * Mathf.Rad2Deg;

                _angle = Mathf.LerpAngle(_angle, angle, 0.05f);
                this.transform.rotation = Quaternion.Euler(new Vector3(0, _angle - 90, 0));
            }
        }
        else
        {
            if (_targetPlayerId != 0)
            {
                this.transform.rotation = Quaternion.Euler(new Vector3(0, _angle - 90, 0));
            }
        }
    }

    public void Stamp1()
    {
        _stampColl.gameObject.SetActive(false);
        _stampFx.gameObject.SetActive(true);
        _stampFx.Play();
        
        if (HasStateAuthority)
        {
            DinoStateHAHAHA = DinoState.Walk;
            _stateCoolTime = Random.Range(3f, 5f);
        }
    }

    public void ShowStamp1Coll()
    {
        _stampColl.gameObject.SetActive(true);
    }

    public void Stamp2()
    {
        if (HasStateAuthority)
        {
            // for (int i = 0; i < 10; i++)
            // {
            //     float angle = Random.Range(0f, 360f);
            //     RpcSpawnStamp2WaveAttack(angle);
            // }

            for (int i = 0; i < 360; i += 10)
            {
                RpcSpawnStamp2WaveAttack(i);
            }
            
            DinoStateHAHAHA = DinoState.Walk;
            _stateCoolTime = Random.Range(3f, 5f);
        }
    }

    public async void OkStopLetsGo()
    {
        float originalSpeed = _animator.speed;
        _animator.speed = 0;

        var originalRot = this.transform.rotation.eulerAngles;
        
        await this.transform.DORotate(new Vector3(30, originalRot.y, originalRot.z), 0.5f);

        _animator.speed = originalSpeed;
    }
    
    public void CallStone()
    {
        if (HasStateAuthority)
        {
            foreach (var player in _callCollList)
            {
                var playerPos = player.transform.position - new Vector3(0, 0.8f, 0); 

                RpcCallStones(playerPos);
            }

            DinoStateHAHAHA = DinoState.Walk;
            _stateCoolTime = Random.Range(3f, 5f);
        }
        
        _callCollList.Clear();
    }

    public void ShowCallStoneColl()
    {
        if (HasStateAuthority)
        {
            foreach (var player in _prefabSpawner.spawnedPlayers)
            {
                RpcSpawnColl(player.Value);   
            }
        }
    }

    public void StopCallColl()
    {
        foreach (var callColl in _callCollList)
        {
            callColl.SecondInfo();
        }
    }

    public void Fire()
    {
        _fireFx.gameObject.SetActive(true);
        _fireFx.Play();
    }

    public void FireStop()
    {
        _fireFx.Stop();
        _fireFx.gameObject.SetActive(false);

        if (HasStateAuthority)
        {
            DinoStateHAHAHA = DinoState.Walk;
            _stateCoolTime = Random.Range(3f, 5f);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcFireAnime()
    {
        _animator.SetTrigger("Fire");   
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcStampAnime()
    {
        _animator.SetTrigger("Stamp");   
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcStamp2Anime()
    {
        _animator.SetTrigger("Stamp2");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSpawnStamp2WaveAttack(float angle)
    {
        Quaternion waveRotation = Quaternion.Euler(-90, angle, 0);
        
        // 각도를 라디안으로 변환
        var angle_radians = math.radians(angle);
        
        // 새로운 위치 계산
        var wow_x = 1 * math.cos(angle_radians);
        var wow_y = 1 * math.sin(angle_radians);

        var wavPos = this.transform.position + new Vector3(wow_x, 0, -wow_y);

        var waveFx = Instantiate(_spawnWavePrefabFx, wavPos, waveRotation);
        var main = waveFx.main;
        main.simulationSpeed = 0.1f;
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcCallAnime()
    {
        _animator.SetTrigger("Call");
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcCallStones(Vector3 pos)
    {
        Quaternion stoneRotation = Quaternion.Euler(-90, Random.Range(0f, 360f), 0);
        var stoneFx = Instantiate(_stoneFx, pos, stoneRotation);
        // var main = stoneFx.main;
        // main.simulationSpeed = 0.5f;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSpawnColl(NetworkObject networkObject)
    {
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        var callColl = Instantiate(_callColl, networkObject.transform.position, rotation);
        callColl.SetInfo(networkObject.transform);
        
        _callCollList.Add(callColl);
    }
}
