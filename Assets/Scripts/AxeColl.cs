using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeColl : MonoBehaviour
{
    [SerializeField] private Axe _axe;
    private List<Collider> _collList = new();

    private void OnTriggerEnter(Collider other)
    {
        if (_collList.Contains(other))
        {
            return;
        }

        _collList.Add(other);
        OnTrigger(other);
    }

    public void OnTrigger(Collider other)
    {
        var dir = other.ClosestPoint(this.transform.position) - transform.position;
        _axe.CutTree(other.ClosestPoint(this.transform.position), dir.normalized, other.GetComponent<Tree>());
    }

    public void ResetCollList()
    {
        _collList.Clear();
    }
}
