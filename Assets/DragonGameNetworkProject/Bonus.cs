using System;
using Fusion;

public class Bonus : NetworkBehaviour
{
    [Networked, Capacity(20)] private NetworkDictionary<PlayerRef, int> PlayerCoins => default;
    
    private static Bonus _instance;
    public static Bonus Instance => _instance;

    private ChangeDetector _changeDetector;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public override void Spawned()
    {
        base.Spawned();

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public void Add(PlayerRef player, int value)
    {
        if (!PlayerCoins.ContainsKey(player))
        {
            PlayerCoins.Add(player, value);
        }
        else
        {
            int currentValue = PlayerCoins.Get(player);
            PlayerCoins.Set(player, currentValue + value);
        }
    }

    private void Update()
    {
        if (_changeDetector == null) // Game has not started, this object has not been spawned!
        {
            return;
        }
        
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(PlayerCoins):
                    UIController.Instance.UpdatePlayerCoins(PlayerCoins, Runner);
                    break;
            }
        }
    }
}
