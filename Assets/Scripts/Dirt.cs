using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Dirt : NetworkBehaviour
{
    [SerializeField] private Outline _outline;

    private void Start()
    {
        _outline.enabled = false;
    }

    public void Looking(bool isLook)
    {
        _outline.enabled = isLook;
    }

}
