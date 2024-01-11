using System;
using Fusion;
using UnityEngine.Events;

namespace DragonGameNetworkProject
{
    public class GameTimer : NetworkBehaviour
    {
        public UnityEvent onGameStart;
        
        
        private static GameTimer _instance;
        public static GameTimer Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }
        
        [Networked] private TickTimer timer { get; set; }

        public override void Spawned()
        {
            base.Spawned();
            if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
            {
                timer = TickTimer.CreateFromSeconds(Runner,11.0f);
            }
        }

        private bool gameStartEventTriggered = false; 
        public override void FixedUpdateNetwork()
        {
            if (timer.RemainingTime(Runner) >= 1.0f)
            {
                UIController.Instance.SetTimerText($"Game will start in { (timer.RemainingTime(Runner) + 1.0f)?.ToString("F2")} seconds");
            }

            if (timer.RemainingTime(Runner) <= 1.0f && !timer.Expired(Runner))
            {
                UIController.Instance.SetTimerText("Go!!");
            }

            if (timer.Expired(Runner) && gameStartEventTriggered == false)
            {
                onGameStart?.Invoke();
                gameStartEventTriggered = true;
            }
        }
    }
}