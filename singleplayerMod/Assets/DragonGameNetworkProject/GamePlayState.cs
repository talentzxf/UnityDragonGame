using System;
using DragonGameNetworkProject.DragonAvatarMovements;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace DragonGameNetworkProject
{
    public class GamePlayState : MonoBehaviour
    {
        public AudioClip titleMusic;
        public AudioClip waitingMusic;
        public AudioClip gameMusic;
        public AudioClip resultMusic;
        
        public float volumn = 0.4f;

        private AudioSource _audioSource;

        public bool gameStarted { set; get; } = false;

        private static GamePlayState _instance;
        public static GamePlayState Instance => _instance;

        public UnityEvent onGamePreparing = new ();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = true;

            _audioSource.clip = titleMusic;
            _audioSource.volume = volumn;
            _audioSource.Play();

            GameTimer.Instance.onGameCompleted.AddListener(() =>
            {
                _audioSource.clip = resultMusic;
                _audioSource.volume = volumn;
                _audioSource.Play();
            });
        }

        //private ChangeDetector _changeDetector;

       

        public void Start()
        {
            // base.Spawned();
            // _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            _audioSource.clip = waitingMusic;
            _audioSource.volume = volumn;
            _audioSource.Play();
        }

        private void Update()
        {
            // if (_changeDetector != null)
            // {
            //     foreach (var change in _changeDetector.DetectChanges(this))
            //     {
            //         switch (change)
            //         {
            //             case nameof(gameStarted):
            //                 if (gameStarted)
            //                 {
            //                     // Hide Prepare UI.
            //                     PrepareUI.Instance.gameObject.SetActive(false);
            //                     
            //                     // Show in game UI.
            //                     UIController.Instance.ActivateUiDocument();
            //
            //                     // Set dragon controller.
            //                     DragonAvatarController.LocalController.SwitchTo<DragonAvatarGroundMovement>();
            //
            //                     Camera.main.GetComponent<FirstPersonCamera>().enabled = true;
            //
            //                     if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
            //                     {
            //                         GameTimer.Instance.StartTimer();
            //                     }
            //
            //                     _audioSource.clip = gameMusic;
            //                     _audioSource.Play();
            //                     
            //                     onGamePreparing?.Invoke();
            //                     // How to close the room so nobody else can join????
            //                 }
            //
            //                 break;
            //         }
            //     }
            // }
        }
    }
}