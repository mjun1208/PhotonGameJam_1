using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Global : MonoBehaviour
{
    public static Global Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType<Global>();
            }
            
            return _instance;
        }
    }

    private static Global _instance = null;

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public string MyName { get; set; }
    public string RoomName { get; set; }
    
    public BasicSpawner BasicSpawner { get; set; }

    public void SetBasicSpawner(BasicSpawner basicSpawner)
    {
        if (BasicSpawner != null)
        {
            Destroy(BasicSpawner.gameObject);
        }
        
        BasicSpawner = basicSpawner;
        BasicSpawner.transform.SetParent(this.transform);

        RoomName = BasicSpawner.RoomName;
    }
}
