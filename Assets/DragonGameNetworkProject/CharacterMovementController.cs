using DragonGameNetworkProject.DragonMovements;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class CharacterMovementController : NetworkBehaviour
    {
        public GameObject avatarGO;

        [Networked] public AbstractMovement currentMovement { set; get; }

        public AbstractMovement prevMovement = null;

        private AbstractMovement[] _movements;

        private AbstractMovement[] movements
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

        public override void Spawned()
        {
            foreach (var movement in movements)
            {
                movement.enabled = false;
            }
        }

        private void RefreshComponents()
        {
            _movements = gameObject.GetComponents<AbstractMovement>();
        }

        public T GetMovement<T>() where T : AbstractMovement
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

        public bool SwitchTo<T>() where T : AbstractMovement
        {
            foreach (var movement in movements)
            {
                if (movement.IsType<T>())
                {
                    movement.enabled = true;

                    if (currentMovement != null && movement != null)
                        Debug.Log("Switched state from " + currentMovement.GetType().Name + " to " +
                                  movement.GetType().Name);

                    currentMovement = movement;

                    if (movement.IsType<DragonLandMovement>())
                    {
                        Debug.Log("Land ???");
                    }

                    return true;
                }
                else
                {
                    movement.enabled = false;
                }
            }

            return false;
        }

        public void FixedUpdate()
        {
            if (prevMovement != currentMovement)
            {
                if (prevMovement != null)
                {
                    prevMovement.OnLeaveMovement();
                }

                if (currentMovement != null)
                {
                    currentMovement.OnEnterMovement();
                }

                prevMovement = currentMovement;
            }

            foreach (var movement in movements)
            {
                if (movement == currentMovement)
                {
                    movement.enabled = true;
                }
                else
                {
                    movement.enabled = false;
                }
            }
        }
    }
}