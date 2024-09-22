using System;
using DG.Tweening;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private Outline _outline;
    private Vector3 _originPos;
    private Quaternion _originRot;
    
    public GameObject Apple;
    public Transform AppleSpawnPoint;

    private void Start()
    {
        _originPos = this.transform.position;
        _originRot = this.transform.rotation;
    }

    public void Look(bool look)
    {
        _outline.enabled = look;
    }

    public void Hit()
    {
        this.transform.DOShakeRotation(0.3f, new Vector3(10f, 0f, 10f)).OnComplete(() =>
        {
            this.transform.rotation = _originRot;
        });
    }
}
