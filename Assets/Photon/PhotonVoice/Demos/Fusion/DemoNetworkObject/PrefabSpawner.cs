#if FUSION_WEAVER
using Fusion;
using Fusion.Sockets;

namespace Photon.Voice.Fusion.Demo
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    public class PrefabSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkObject lobbyPlayerInfo;
        [SerializeField]
        private NetworkObject prefab;

        public Dictionary<PlayerRef, NetworkObject> spawnedPlayers { get; set; } = new Dictionary<PlayerRef, NetworkObject>();
        public Dictionary<PlayerRef, NetworkObject> spawnedPlayerOb { get; set; } = new Dictionary<PlayerRef, NetworkObject>();
        public Dictionary<PlayerRef, NetworkRunner> playerRunners { get; set; } = new Dictionary<PlayerRef, NetworkRunner>();

        [SerializeField]
        private bool debugLogs;


        private void Awake()
        {
            Global.Instance.PrefabSpawner = this;
        }

        #region INetworkRunnerCallbacks

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (this.debugLogs)
            {
                Debug.Log($"OnPlayerJoined {player} mode = {runner.GameMode}");
            }
            switch (runner.GameMode)
            {
                case GameMode.Single:
                case GameMode.Server:
                case GameMode.Host:
                    SelectGOGO(runner, player);
                    playerRunners.Add(player, runner);
                    // this.SpawnPlayer(runner, player);
                    break;
            }
        }

        public void SelectGOGO(NetworkRunner runner, PlayerRef player)
        {
            NetworkObject instance = runner.Spawn(this.lobbyPlayerInfo, Vector3.zero, Quaternion.identity, player);
            instance.GetComponent<LobbyPlayerInfo>().SetPlayerRef(player);
            this.spawnedPlayers[player] = instance;
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (this.debugLogs)
            {
                Debug.Log($"OnPlayerLeft {player} mode = {runner.GameMode}");
            }
            switch (runner.GameMode)
            {
                case GameMode.Single:
                case GameMode.Server:
                case GameMode.Host:
                    this.TryDespawnPlayer(runner, player);
                    if (playerRunners.ContainsKey(player))
                    {
                        playerRunners.Remove(player);
                    }

                    break;
            }
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (this.debugLogs)
            {
                Debug.Log($"OnShutdown mode = {runner.GameMode} reason = {shutdownReason}");
                foreach (var pair in this.spawnedPlayers)
                {
                    Debug.LogWarning($"Prefab not despawned? {pair.Key}:{pair.Value?.Id}");
                }
            }
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
        {
            if (this.debugLogs)
            {
                Debug.Log($"OnConnectedToServer mode = {runner.GameMode}");
            }
            if (runner.GameMode == GameMode.Shared)
            {
                // this.SpawnPlayer(runner, runner.LocalPlayer);
            }
        }

#if FUSION2
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
#else
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
#endif
        {
            if (this.debugLogs)
            {
                Debug.Log($"OnDisconnectedFromServer mode = {runner.GameMode}");
            }
            if (runner.GameMode == GameMode.Shared)
            {
                // this.TryDespawnPlayer(runner, runner.LocalPlayer);
            }
        }

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }


        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
        {
        }
#if FUSION2

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey reliableKey, ArraySegment<byte> data)
        {
        }

        void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey reliableKey, float progress)
        {
        }
#else
        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }
#endif
        #endregion

        [SerializeField] private KingDino _kingDino;
        
        public void SpawnPlayer(PlayerRef player, PlayerType playerType)
        {
            playerRunners.TryGetValue(player, out var runner);

            NetworkObject instance = runner.Spawn(this.prefab, Vector3.zero, Quaternion.identity, player);
            if (this.debugLogs)
            {
                if (this.spawnedPlayers.TryGetValue(player, out NetworkObject oldValue))
                {
                    Debug.LogWarning($"Replacing NO {oldValue?.Id} w/ {instance?.Id} for {player}");
                }
                else
                {
                    Debug.Log($"Spawned NO {instance?.Id} for {player}");
                }
            }
            
            spawnedPlayerOb.Add(player, instance);
            
            var playerComponent = instance.GetComponent<Player>();
            playerComponent.SetPlayerRef(player);
            playerComponent.SetPlayerType(playerType);
            playerComponent.OnChangedPlayerType();
            // this.spawnedPlayers[player] = instance;
            
            if (runner.IsServer && player.PlayerId == 1)
            {
                SpawnDino(runner);
            }
        }

        private void SpawnDino(NetworkRunner runner)
        {
            var dino = runner.Spawn(_kingDino, new Vector3(-6f, 0, -38f), Quaternion.identity);
            dino.SetPrefabSpawner(this);
        }

        private bool TryDespawnPlayer(NetworkRunner runner, PlayerRef player)
        {
            if (this.spawnedPlayers.TryGetValue(player, out NetworkObject instance))
            {
                if (this.debugLogs)
                {
                    Debug.Log($"Despawning NO {instance?.Id} for {player}");
                }
                runner.Despawn(instance);
                return this.spawnedPlayers.Remove(player);
            }
            if (this.debugLogs)
            {
                Debug.LogWarning($"No spawned NO found for player {player}");
            }
            return false;
        }
    }
}
#endif
