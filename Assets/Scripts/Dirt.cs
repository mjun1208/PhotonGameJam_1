using System;
using Fusion;
using UnityEngine;

public class Dirt : NetworkBehaviour
{
    [SerializeField] private Outline _outline;
    [SerializeField] private GameObject plant;
    [SerializeField] private GameObject plant2;
    [SerializeField] private GameObject FX;
    [SerializeField] private GameObject FX_grow;
    
    [Networked, OnChangedRender(nameof(OnChangePlated))] 
    public NetworkBool Planted { get; set; }
    
    [Networked, OnChangedRender(nameof(OnChangeGrew))] 
    public NetworkBool Grew { get; set; }

    private float _growDelay;
    private bool _growDelayTimerOn = false;

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
