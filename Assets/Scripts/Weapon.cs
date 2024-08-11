using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private KingDino _kingDino;
    public KingDino KingDino => _kingDino;

    private List<KingDino> _hitList = new List<KingDino>();
    private Vector3 hitPos;

    private bool _hitOn = false;

    public GameObject SwordImpactBlue;
    public GameObject Effectss;
    public GameObject HitPo;

    public void aaOnTriggerStay(Collider other)
    {
        _kingDino = other.GetComponent<KingDino>();
    }

    private void LateUpdate()
    {
        _kingDino = null;
    }

    public void HitClear()
    {
        _hitList.Clear();
    }

    public void HitOn(bool on)
    {
        _hitOn = on;
    }

    private void Update()
    {
        if (_hitOn)
        {
            Hit();
        }
    }

    private void Hit()
    {
        if (_kingDino == null)
        {
            return;
        }
        
        if (_hitList.Contains(_kingDino))
        {
            return;
        }
        
        _hitList.Add(_kingDino);
        _kingDino.Damaged(40);
        
        GameObject.Instantiate(SwordImpactBlue, HitPo.transform.position, Quaternion.identity);
    }
}
