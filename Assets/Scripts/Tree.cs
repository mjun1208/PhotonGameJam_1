using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private Outline _outline;
    
    public void Look(bool look)
    {
        _outline.enabled = look;
    }
}
