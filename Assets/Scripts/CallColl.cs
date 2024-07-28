using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallColl : MonoBehaviour
{
    private Transform _followTransform;

    public void SetInfo(Transform followTransform)
    {
        _followTransform = followTransform;
    }
    
    public void SecondInfo()
    {
        _followTransform = null;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (_followTransform != null)
        {
            this.transform.position = new Vector3(_followTransform.transform.position.x,
                _followTransform.transform.position.y + 0.8f, _followTransform.transform.position.z);
        }
    }
}
