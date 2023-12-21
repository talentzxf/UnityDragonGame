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

        private AbstractCharacterMovement[] _movements;

        private AbstractCharacterMovement[] movements
        {
            get
            {
                if (_movements == null)
                {
                    RefreshComponents();
                }
                return _movements;
            }
        }

        private void RefreshComponents()
        {
            _movements = gameObject.GetComponents<AbstractCharacterMovement>();
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

            T newMovement = gameObject.AddComponent<T>();

            RefreshComponents();
            return newMovement;
        }

        public void SwitchTo<T>() where T : AbstractCharacterMovement
        {
            bool movementFound = false;
            foreach (var movement in movements)
            {
                if (movement.IsType<T>())
                {
                    movement.enabled = true;
                    movementFound = true;
#if UNITY_EDITOR
                    currentEnabled = movement;
#endif
                }
                else
                {
                    movement.enabled = false;
                }
            }

            if (!movementFound)
            {
                T newMovement = gameObject.AddComponent<T>();
                newMovement.enabled = true;
                RefreshComponents();
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