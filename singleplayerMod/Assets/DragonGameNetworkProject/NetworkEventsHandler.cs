using System;
using System.Collections.Generic;
// using Fusion;
// using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Events;

public class NetworkEventsHandler : MonoBehaviour
{
    // public static readonly UnityEvent<NetworkRunner, string> LocalPlayerJoined = new();
    // public static readonly UnityEvent<NetworkRunner, PlayerRef> PlayerJoined = new();
    // public static readonly UnityEvent<PlayerRef> PlayerLeft = new(); 
    // public static readonly UnityEvent<string> ServerDisconnected = new();
    // public static readonly UnityEvent<string> ConnectFailed = new();
    // public static readonly UnityEvent<NetworkRunner> SceneLoadDone = new();
    // public static readonly UnityEvent SceneLoadStart = new();
    // public static readonly UnityEvent HostMigrated = new(); // Seems this will never trigger in shared host mode.
    // public static readonly UnityEvent SelectedAsMasterClient = new();
    //
    // public static readonly UnityEvent<NetworkRunner> ServerConnected = new();
    //
    // private NetworkRunner runner;
    //
    // private bool isMasterClient = false;
    //
    // private void Update()
    // {
    //     if (runner == null)
    //         return;
    //     if (runner.State != NetworkRunner.States.Running)
    //         return;
    //
    //     if (!isMasterClient && runner.IsSharedModeMasterClient)
    //     {
    //         Debug.Log("I'm Master Client now!");
    //         isMasterClient = true;
    //         
    //         SelectedAsMasterClient?.Invoke();
    //     }
    // }
    //
    // public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    // {
    // }
    //
    // public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    // {
    // }
    //
    // public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    // {
    //     string playerId = runner.GetPlayerUserId(player);
    //     if (player == runner.LocalPlayer)
    //         LocalPlayerJoined?.Invoke(runner, playerId);
    //     PlayerJoined?.Invoke(runner, player);
    // }
    //
    // public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    // {
    //     PlayerLeft?.Invoke(player);
    // }
    //
    // public void OnInput(NetworkRunner runner, NetworkInput input)
    // {
    // }
    //
    // public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    // {
    // }
    //
    // public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    // {
    // }
    //
    // public void OnConnectedToServer(NetworkRunner runner)
    // {
    //     ServerConnected?.Invoke(runner);
    //
    //     this.runner = runner;
    // }
    //
    // public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    // {
    //     ServerDisconnected?.Invoke(Enum.GetName(typeof(NetDisconnectReason), reason));
    // }
    //
    // public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    // {
    // }
    //
    // public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    // {
    //     ConnectFailed?.Invoke(Enum.GetName(typeof(NetConnectFailedReason), reason));
    // }
    //
    // public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    // {
    // }
    //
    // public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    // {
    // }
    //
    // public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    // {
    // }
    //
    // public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    // {
    //     HostMigrated?.Invoke();
    // }
    //
    // public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    // {
    // }
    //
    // public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    // {
    // }
    //
    // public void OnSceneLoadDone(NetworkRunner runner)
    // {
    //     SceneLoadDone?.Invoke(runner);
    // }
    //
    // public void OnSceneLoadStart(NetworkRunner runner)
    // {
    //     SceneLoadStart?.Invoke();
    // }
}