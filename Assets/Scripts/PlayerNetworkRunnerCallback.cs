using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNetworkRunnerCallback : MonoBehaviour, INetworkRunnerCallbacks
{
    private bool _mouseButton0;
    private bool _mouseButton1;

    // private void Update()
    // {
    //     _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
    //     
    //     _mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);
    // }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        
        Debug.Log(runner.IsPlayer);

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        data.lookDelta += new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));

        if (Input.GetKey(KeyCode.Space))
        {
            data.jump = true;
        }
        else
        {
            data.jump = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            data.buttons.Set(NetworkInputData.MOUSEBUTTON0, true);
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            data.buttons.Set(NetworkInputData.MOUSEBUTTON1, true);
        }
        
        // data.buttons.Set(NetworkInputData.MOUSEBUTTON0, _mouseButton0);
        // _mouseButton0 = false;
        // 
        // data.buttons.Set(NetworkInputData.MOUSEBUTTON1, _mouseButton1);
        // _mouseButton1 = false;
        
        input.Set(data);
    }
    

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}
