using ExitGames.Client.Photon.StructWrapping;
using Fusion;

namespace DragonGameNetworkProject
{
    public class CharacterMovementController : NetworkBehaviour
    {
        [Networked] public AbstractCharacterMovement currentMovement { set; get; }

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
        
        public override void Spawned()
        {
            foreach (var movement in movements)
            {
                movement.enabled = false;
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

        private void Update()
        {
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