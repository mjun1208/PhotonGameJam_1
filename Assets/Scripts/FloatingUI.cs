using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector3 _offset = new Vector3(0, 180, 0);
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(_mainCamera.transform);
        this.transform.Rotate(_offset);
    }
}
