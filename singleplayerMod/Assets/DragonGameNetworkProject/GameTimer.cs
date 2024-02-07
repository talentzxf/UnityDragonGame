using System;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

namespace DragonGameNetworkProject
{
    public class GameTimer : MonoBehaviour
    {
        public UnityEvent onGameStart;
        public UnityEvent onGameCompleted;

        [SerializeField] private AudioClip coutdownBeep;
        [SerializeField] private float countDownTime = 5.0f;
        [SerializeField] private float goTime = 1.0f;
        private AudioSource _countDownAC;
        
        private static GameTimer _instance;
        public static GameTimer Instance => _instance;

        public int TotalTime = 120;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            
            _countDownAC = gameObject.AddComponent<AudioSource>();
            _countDownAC.loop = false;
            _countDownAC.clip = coutdownBeep;
        }
        
      //  [Networked] private TickTimer waitingTimer { get; set; }
        
       // [Networked] private TickTimer gameTimer { get; set; }

        public void StartTimer()
        {
            // if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
            // {
            //    waitingTimer = TickTimer.CreateFromSeconds(Runner,countDownTime + goTime);
            // }
        }

        private bool gameStartEventTriggered = false;
        private bool gameCompletEventTriggered = false;

        private void BeepIfLessThen(int upperBound, float remainTime)
        {
            int integralPart = Mathf.CeilToInt(remainTime);
            if (integralPart <= upperBound)
            {
                float fractalPart = remainTime - integralPart;

                if (fractalPart <= Mathf.Epsilon)
                {
                    if(!_countDownAC.isPlaying)
                        _countDownAC.Play();
                }
            }
        }
        
        private void Update()
        {
            // if (Runner == null || Runner.State != NetworkRunner.States.Running)
            //     return;
            //
            // if (!waitingTimer.IsRunning)
            //     return;
            //
            // float remainTime = waitingTimer.RemainingTime(Runner) ?? Mathf.Infinity;
            // if (remainTime >= 1.0f)
            // {
            //     UIController.Instance.SetTimerText($"Game will start in { (waitingTimer.RemainingTime(Runner) - 1.0f)?.ToString("F2")} seconds");
            //
            //     BeepIfLessThen(3, remainTime);
            // }
            //
            // if (remainTime <= 1.0f && !waitingTimer.Expired(Runner))
            // {
            //     UIController.Instance.SetTimerText("Go!!");
            //
            //     if (!gameStartEventTriggered)
            //     {
            //         onGameStart?.Invoke();
            //         gameStartEventTriggered = true;
            //         
            //         if(Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
            //             gameTimer = TickTimer.CreateFromSeconds(Runner, TotalTime);
            //     }
            // }
            //
            // if (gameStartEventTriggered && waitingTimer.Expired(Runner))
            // {
            //     if (!gameTimer.Expired(Runner))
            //     {
            //         UIController.Instance.SetTimerText($"Remaining Time:{gameTimer.RemainingTime(Runner)?.ToString("F2")} seconds");
            //         BeepIfLessThen(3, gameTimer.RemainingTime(Runner)??Mathf.Infinity);
            //     }
            //     else
            //     {
            //         if (!gameCompletEventTriggered)
            //         {
            //             onGameCompleted?.Invoke();
            //             gameCompletEventTriggered = true;
            //         }
            //     }
            // }
        }
    }
}