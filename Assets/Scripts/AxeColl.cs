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
                
        if (other.GetComponent<Animal>() != null)
        {
            var dira = other.ClosestPoint(this.transform.position) - transform.position;
            _axe.CutAnimal(other.ClosestPoint(this.transform.position), dira.normalized, other.GetComponent<Animal>());
            
            return;
        }

        if (other.GetComponent<Tree>() == null)
        {
            return;
        }
        
        var dir = other.ClosestPoint(this.transform.position) - transform.position;
        _axe.CutTree(other.ClosestPoint(this.transform.position), dir.normalized, other.GetComponent<Tree>());
    }

    public void ResetCollList()
    {
        _collList.Clear();
    }
}
