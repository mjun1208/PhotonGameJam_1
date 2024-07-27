using System;
using System.Linq;
using Fusion;
using Photon.Voice.Fusion.Demo;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class KingDino : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _fireFx;
    
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
    }

    public void SetPrefabSpawner(PrefabSpawner prefabSpawner)
    {
        _prefabSpawner = prefabSpawner;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (_targetPlayerId == 0 && HasStateAuthority)
            {
                GetTarget();
            }
        }
    }

    private void ChangeState()
    {
        if (_stateCoolTime <= 0)
        {
            int enumCount = Enum.GetValues(typeof(DinoState)).Length;
            DinoStateHAHAHA = (DinoState) Random.Range(1, enumCount);
            
            //
            // var enumList = Enum.GetValues(typeof(DinoState)).Cast<DinoState>().ToList();
            //
            // // 중복 제거
            // enumList.Remove(DinoStateHAHAHA);
            // DinoStateHAHAHA = (DinoState)
                
            switch (DinoStateHAHAHA)
            {
                case DinoState.Fire:
                {
                    RpcFireAnime();
                    break;
                }
                case DinoState.Stamp:
                {
                    RpcStampAnime();
                    break;
                }
            }

            _stateCoolTime = Random.Range(3f, 5f);
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
                move *= 2f;
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

                _angle = Mathf.LerpAngle(_angle, angle, 0.005f);
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
        if (HasStateAuthority)
        {
            DinoStateHAHAHA = DinoState.Walk;
        }
    }
    
    public void Stamp2()
    {
        if (HasStateAuthority)
        {
            DinoStateHAHAHA = DinoState.Walk;
        }
    }
    
    public void CallStone()
    {
        
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
}
