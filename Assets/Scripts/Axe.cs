using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private AxeColl _axeColl;

    [SerializeField] private Renderer _renderer;
    [SerializeField] private List<Material> _setMaterial;

    public void SetMaterial(InventoryItemType itemType)
    {
        if (itemType == InventoryItemType.Axe)
        {
            _renderer.material = _setMaterial[0];
        }
        if (itemType == InventoryItemType.Axe_1)
        {
            _renderer.material = _setMaterial[1];
        }
        if (itemType == InventoryItemType.Axe_2)
        {
            _renderer.material = _setMaterial[2];
        }
    }
    
    public void CutTree(Vector3 hitPosition, Vector3 dir, Tree targetTree)
    {
        _player.SpawnLog(hitPosition, dir, targetTree);
    }
    
    public void CutAnimal(Vector3 hitPosition, Vector3 dir, Animal targetAnimal)
    {
        _player.SpawnMeat(hitPosition, dir, targetAnimal);
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
