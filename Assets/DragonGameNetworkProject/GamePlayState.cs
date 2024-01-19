using DragonGameNetworkProject.DragonAvatarMovements;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace DragonGameNetworkProject
{
    public class GamePlayState : NetworkBehaviour
    {
        public AudioClip titleMusic;
        public AudioClip waitingMusic;
        public AudioClip gameMusic;
        public AudioClip resultMusic;

        private AudioSource _audioSource;
        
        [Networked] public bool gameStarted { set; get; } = false;

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
            _audioSource.Play();
            
            GameTimer.Instance.onGameCompleted.AddListener(() =>
            {
                _audioSource.clip = resultMusic;
                _audioSource.Play();
            });
        }

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            base.Spawned();
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            _audioSource.clip = waitingMusic;
            _audioSource.Play();
        }

        private void Update()
        {
            if (_changeDetector != null)
            {
                foreach (var change in _changeDetector.DetectChanges(this))
                {
                    switch (change)
                    {
                        case nameof(gameStarted):
                            if (gameStarted)
                            {
                                // Hide Prepare UI.
                                PrepareUI.Instance.gameObject.SetActive(false);
                                
                                // Show in game UI.
                                UIController.Instance.ActivateUiDocument();

                                // Set dragon controller.
                                DragonAvatarController.LocalController.SwitchTo<DragonAvatarGroundMovement>();

                                Camera.main.GetComponent<FirstPersonCamera>().enabled = true;

                                if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
                                {
                                    GameTimer.Instance.StartTimer();
                                }

                                _audioSource.clip = gameMusic;
                                _audioSource.Play();
                                
                                onGamePreparing?.Invoke();
                                // How to close the room so nobody else can join????
                            }

                            break;
                    }
                }
            }
        }
    }
}