using UnityEngine;

public class Missile : MonoBehaviour
{
    public int Damage = 1;
    public GameObject HitFx;

    private Vector3 _dir;

    public void Go(Vector3 dir)
    {
        _dir = dir;
    }

    public void FixedUpdate()
    {
        this.transform.position += _dir * 20f * Time.deltaTime;
    }
}
