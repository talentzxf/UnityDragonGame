using System;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class CharacterMovementController : NetworkBehaviour
    {
        public AbstractCharacterMovement currentMovement;

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
            return default;
        }

        public void SwitchTo<T>() where T : AbstractCharacterMovement
        {
            foreach (var movement in movements)
            {
                if (movement.IsType<T>())
                {
                    movement.enabled = true;
                    currentMovement = movement;
                }
                else
                {
                    movement.enabled = false;
                }
            }
        }
        
        // private void Update()
        // {
        //     foreach (var movement in movements)
        //     {
        //         if (movement.enabled)
        //         {
        //             currentEnabled = movement;
        //         }
        //     }
        // }
    }
}