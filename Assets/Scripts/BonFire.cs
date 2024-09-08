using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BonFire : NetworkBehaviour
{
    [SerializeField] private Outline _outline;
    
    public void Look(bool look)
    {
        _outline.enabled = look;
    }
}
