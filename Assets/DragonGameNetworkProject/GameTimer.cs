using System;
using Fusion;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

namespace DragonGameNetworkProject
{
    public class GameTimer : NetworkBehaviour
    {
        public UnityEvent onGameStart;

        [SerializeField] private float countDownTime = 5.0f;
        [SerializeField] private float goTime = 1.0f;
        
        private static GameTimer _instance;
        public static GameTimer Instance => _instance;

        public int TotalTime = 300;

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

        public void StartTimer()
        {
            if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
            {
                timer = TickTimer.CreateFromSeconds(Runner,countDownTime + goTime);
            }
        }

        private bool gameStartEventTriggered = false; 
        
        private void Update()
        {
            if (Runner == null || Runner.State != NetworkRunner.States.Running)
                return;
            
            if (!timer.IsRunning)
                return;
            
            if (timer.RemainingTime(Runner) >= 1.0f)
            {
                UIController.Instance.SetTimerText($"Game will start in { (timer.RemainingTime(Runner) - 1.0f)?.ToString("F2")} seconds");
            }

            if (timer.RemainingTime(Runner) <= 1.0f && !timer.Expired(Runner))
            {
                UIController.Instance.SetTimerText("Go!!");

                if (!gameStartEventTriggered)
                {
                    onGameStart?.Invoke();
                    gameStartEventTriggered = true;
                }
            }
        }
    }
}