using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{ 
    //private Dictionary<PlayerRef, int> PlayerCoins => default;
    private object CoinLock = new();

    private static Bonus _instance;
    public static Bonus Instance => _instance;

  //  private ChangeDetector _changeDetector;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }
    
    private bool spawned = false;

    // public override void Spawned()
    // {
    //     base.Spawned();
    //
    //     _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    //
    //     NetworkEventsHandler.PlayerJoined.AddListener((runner, playerRef) =>
    //     {
    //         if (PlayerCoins.ContainsKey(playerRef))
    //         {
    //             Debug.LogError("The user is already in the list, but re-joined?");
    //         }
    //         else
    //         {
    //             AddPlayerCoinRpc(playerRef, 0);
    //         }
    //     });
    //
    //     NetworkEventsHandler.PlayerLeft.AddListener(playerRef => { PlayerCoins.Remove(playerRef); });
    //     spawned = true;
    // }

    // public int GetCoinCount(PlayerRef playerRef)
    // {
    //     lock (CoinLock)
    //     {
    //         if (!PlayerCoins.ContainsKey(playerRef))
    //         {
    //             PlayerCoins.Add(playerRef, 0);
    //         }
    //         return PlayerCoins.Get(playerRef);            
    //     }
    // }

    
    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    // public void AddPlayerCoinRpc(PlayerRef player, int value)
    // {
    //     lock (CoinLock)
    //     {
    //         if (!PlayerCoins.ContainsKey(player))
    //         {
    //             if (value > 0)
    //                 PlayerCoins.Add(player, value);
    //             if (value <= 0)
    //                 PlayerCoins.Add(player, 0);
    //         }
    //         else
    //         {
    //             int currentValue = PlayerCoins.Get(player);
    //             if (currentValue + value > 0)
    //                 PlayerCoins.Set(player, currentValue + value);
    //             else
    //                 PlayerCoins.Set(player, 0);
    //         }
    //     }
    // }

    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    // public void AddCoinFromToRpc(PlayerRef from, int fromCount, PlayerRef to, int toCount)
    // {
    //     lock(CoinLock)
    //     {
    //         int currentCoinCount = GetCoinCount(from);
    //         if (currentCoinCount + fromCount > 0.0f)
    //         {
    //             AddPlayerCoinRpc(to, toCount);
    //         }
    //             
    //         AddPlayerCoinRpc(from, fromCount);            
    //     }
    // }

    private void Update()
    {
        // if(spawned)
        //     UIController.Instance.UpdatePlayerCoins(PlayerCoins, Runner);
    }
}