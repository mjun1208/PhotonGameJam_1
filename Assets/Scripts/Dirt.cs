using Fusion;
using UnityEngine;

public class Dirt : NetworkBehaviour
{
    [SerializeField] private Outline _outline;
    [SerializeField] private GameObject plant;
    
    [Networked, OnChangedRender(nameof(OnChangePlated))] 
    public NetworkBool Planted { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        if (Planted)
        {
            plant.SetActive(Planted);
        }
    }

    private void Start()
    {
        _outline.enabled = false;
    }

    public void Looking(bool isLook)
    {
        _outline.enabled = isLook;
    }

    private void OnChangePlated()
    {
        plant.SetActive(Planted);
    }

    // [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void GoPlated()
    {
        Planted = true;
        // OnChangePlated();
    }
}
