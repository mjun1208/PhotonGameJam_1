using System;
using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DummyBasicSpawner : BasicSpawner 
{
    [SerializeField]
    private NetworkRunner _dumymRunnerPrefab;
    private NetworkRunner _dummyRunner;
    
    async void StartGame(GameMode mode)
    {
        Global.Instance.MyName = "testMYMY";
        RoomName = "test";

        Global.Instance.SetBasicSpawner(this);

        // Create the Fusion runner and let it know that we will be providing user input
        _dummyRunner = Instantiate(_dumymRunnerPrefab);
        _dummyRunner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        
        // Start or join (depends on gamemode) a session with a specific name
        var result = await _dummyRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = RoomName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 4,
        });

        if (result.Ok == false)
        {
            Global.Instance.RoomEnterFail();
            return;
        }

        Global.Instance.RoomName = _dummyRunner.SessionInfo.Name;
    }
    
    private void OnGUI()
    {
        if (_dummyRunner == null)
        {
            if (GUI.Button(new Rect(0,0,200,40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0,40,200,40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
}
