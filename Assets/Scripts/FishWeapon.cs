using Fusion;
using UnityEngine;

public class FishWeapon : NetworkBehaviour
{
    [SerializeField] private Outline _outline;
    [SerializeField] private Rigidbody _rigidbody;

    public void Fished()
    {
        _rigidbody.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

    public void Look(bool look)
    {
        _outline.enabled = look;
    }
}
