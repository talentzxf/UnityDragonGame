using System;
using DragonGameNetworkProject.DragonAvatarMovements;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class GamePlayState : NetworkBehaviour
    {
        [Networked] public bool gameStarted { set; get; } = false;

        private static GamePlayState _instance;
        public static GamePlayState Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            base.Spawned();
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
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
                                // Hide UI.
                                PrepareUI.Instance.gameObject.SetActive(false);
                                UIController.Instance.ActivateUiDocument();

                                // Set dragon controller.
                                DragonAvatarController.LocalController.SwitchTo<DragonAvatarGroundMovement>();

                                Camera.main.GetComponent<FirstPersonCamera>().enabled = true;

                                if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
                                {
                                    GameTimer.Instance.StartTimer();
                                }
                            }

                            break;
                    }
                }
            }
        }
    }
}