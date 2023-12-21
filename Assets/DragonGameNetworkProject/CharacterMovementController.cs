using System;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class CharacterMovementController : MonoBehaviour
    {
        public AbstractCharacterMovement currentEnabled;
        
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
                    currentEnabled = movement;
                }
                else
                {
                    movement.enabled = false;
                }
            }
        }

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
    }
}