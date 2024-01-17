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
        public UnityEvent onGameCompleted;

        [SerializeField] private float countDownTime = 5.0f;
        [SerializeField] private float goTime = 1.0f;
        
        private static GameTimer _instance;
        public static GameTimer Instance => _instance;

        public int TotalTime = 5;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }
        
        [Networked] private TickTimer waitingTimer { get; set; }
        
        [Networked] private TickTimer gameTimer { get; set; }

        public void StartTimer()
        {
            if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
            {
                waitingTimer = TickTimer.CreateFromSeconds(Runner,countDownTime + goTime);
            }
        }

        private bool gameStartEventTriggered = false; 
        
        private void Update()
        {
            if (Runner == null || Runner.State != NetworkRunner.States.Running)
                return;
            
            if (!waitingTimer.IsRunning)
                return;
            
            if (waitingTimer.RemainingTime(Runner) >= 1.0f)
            {
                UIController.Instance.SetTimerText($"Game will start in { (waitingTimer.RemainingTime(Runner) - 1.0f)?.ToString("F2")} seconds");
            }

            if (waitingTimer.RemainingTime(Runner) <= 1.0f && !waitingTimer.Expired(Runner))
            {
                UIController.Instance.SetTimerText("Go!!");

                if (!gameStartEventTriggered)
                {
                    onGameStart?.Invoke();
                    gameStartEventTriggered = true;
                    
                    if(Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
                        gameTimer = TickTimer.CreateFromSeconds(Runner, TotalTime);
                }
            }

            if (gameStartEventTriggered && waitingTimer.Expired(Runner))
            {
                if (!gameTimer.Expired(Runner))
                {
                    UIController.Instance.SetTimerText($"Remaining game time:{gameTimer.RemainingTime(Runner)?.ToString("F2")} seconds");
                }
                else
                {
                    onGameCompleted?.Invoke();
                }
            }
        }
    }
}