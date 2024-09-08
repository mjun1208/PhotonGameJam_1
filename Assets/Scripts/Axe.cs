using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private AxeColl _axeColl;

    public void CutTree(Vector3 hitPosition, Vector3 dir, Tree targetTree)
    {
        _player.SpawnLog(hitPosition, dir, targetTree);
    }

    public void ResetCollList()
    {
        _axeColl.ResetCollList();
    }

    public void CollOn()
    {
        _axeColl.gameObject.SetActive(true);
        ResetCollList();
    }

    public void CollOff()
    {
        _axeColl.gameObject.SetActive(false);
        ResetCollList();
    }
}
