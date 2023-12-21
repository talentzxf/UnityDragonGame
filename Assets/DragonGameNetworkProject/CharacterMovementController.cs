using System;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class CharacterMovementController : NetworkBehaviour
    {
#if UNITY_EDITOR
        public AbstractCharacterMovement currentEnabled;
#endif

        private AbstractCharacterMovement[] movements;

        private void Awake()
        {
            movements = GetComponents<AbstractCharacterMovement>();
        }

        public T GetMovement<T>() where T : AbstractCharacterMovement
        {
            foreach (var movement in movements)
            {
                if (movement.IsType<T>())
                {
                    return movement as T;
                }
            }

            return default;
        }

        public void SwitchTo<T>() where T : AbstractCharacterMovement
        {
            foreach (var movement in movements)
            {
                if (movement.IsType<T>())
                {
                    movement.enabled = true;
#if UNITY_EDITOR
                    currentEnabled = movement;
#endif
                }
                else
                {
                    movement.enabled = false;
                }
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            foreach (var movement in movements)
            {
                if (movement.enabled)
                {
                    currentEnabled = movement;
                }
            }
        }
#endif
    }
}