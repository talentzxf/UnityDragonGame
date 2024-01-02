using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Events;

public class NetworkEventsHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    public static UnityEvent<string> LocalPlayerJoined = new();
    public static UnityEvent<string> PlayerJoined = new();
    public static UnityEvent<string> PlayerLeft = new(); 
    public static UnityEvent<string> ServerDisconnected = new();
    public static UnityEvent<string> ConnectFailed = new();
    public static UnityEvent SceneLoadDone = new();
    public static UnityEvent SceneLoadStart = new();

    public static UnityEvent<NetworkRunner> ServerConnected = new();

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        string playerId = runner.GetPlayerUserId(player);
        if (player == runner.LocalPlayer)
            LocalPlayerJoined?.Invoke( playerId);
        PlayerJoined?.Invoke(playerId);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        string playerId = runner.GetPlayerUserId(player);
        PlayerLeft?.Invoke(playerId);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
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
        ServerConnected?.Invoke(runner);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        ServerDisconnected?.Invoke(Enum.GetName(typeof(NetDisconnectReason), reason));
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        ConnectFailed?.Invoke(Enum.GetName(typeof(NetConnectFailedReason), reason));
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
        SceneLoadDone?.Invoke();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        SceneLoadStart?.Invoke();
    }
}