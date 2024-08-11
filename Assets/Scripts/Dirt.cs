using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class Dirt : NetworkBehaviour
{
    [SerializeField] private Outline _outline;
    [SerializeField] private GameObject plant;
    [SerializeField] private GameObject plant2;
    [SerializeField] private GameObject FX;
    [SerializeField] private GameObject FX_grow;
    
    [SerializeField] private GameObject DestoryFx;
    
    [SerializeField] private Missile missile;
    
    [SerializeField] private List<Transform> missileSpawnPoint;

    [Networked, OnChangedRender(nameof(OnChangePlated))] 
    public NetworkBool Planted { get; set; }
    
    [Networked, OnChangedRender(nameof(OnChangeGrew))] 
    public NetworkBool Grew { get; set; }

    private float _growDelay;
    private bool _growDelayTimerOn = false;

    private bool _shoot = false;

    public override void Spawned()
    {
        base.Spawned();
        if (Planted)
        {
            plant.SetActive(Planted);
        }

        if (Grew)
        {
            plant.SetActive(false);
            plant2.SetActive(true);
        }

        var fx = GameObject.Instantiate(FX, this.transform);
        fx.transform.localScale *= 3;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSpawnMissile()
    {
        if (Global.Instance.KingDino == null)
        {
            return;
        }
        
        SpawnMissile();
    }

    public async void SpawnMissile()
    {
        for (int j = 0; j < 2; j++)
        {
            await UniTask.Delay(300);
         
            for (int i = 0; i < 3; i++)
            {
                var pos = missileSpawnPoint[i].position;

                var kingPos = Global.Instance.KingDino.MissileTarget.transform.position;
                var dir = kingPos - pos;
                dir = dir.normalized;

                var spawnMissile = GameObject.Instantiate(missile, missileSpawnPoint[i].position, Quaternion.identity);
                spawnMissile.transform.LookAt(kingPos);
                spawnMissile.Go(dir);
            }
        }

        await UniTask.Delay(1000);
        
        Instantiate(FX ,this.transform.position, Quaternion.identity);
        Instantiate(DestoryFx ,this.transform.position, Quaternion.identity);
        
        this.gameObject.SetActive(false);

        await UniTask.Delay(1000);
        
        if (HasStateAuthority)
        {
            Runner.Despawn(this.GetComponent<NetworkObject>());
        }
    }
    
    private void Start()
    {
        _outline.enabled = false;
    }

    private void LateUpdate()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (Planted && !Grew && _growDelayTimerOn)
        {
            if (_growDelay > 0f)
            {
                _growDelay -= Time.deltaTime;
            }
            else
            {
                Grew = true;

                if (!_shoot)
                {
                    _shoot = true;
                    RpcSpawnMissile();
                }
            }
        }
    }

    public void Looking(bool isLook)
    {
        _outline.enabled = isLook;
    }

    private void OnChangePlated()
    {
        plant.SetActive(Planted);

        if (HasStateAuthority)
        {
            _growDelay = 5f;
            _growDelayTimerOn = true;
        }
    }    
    
    private void OnChangeGrew()
    {
        plant.SetActive(false);
        plant2.SetActive(true);
        FX_grow.SetActive(true);
    }
}
