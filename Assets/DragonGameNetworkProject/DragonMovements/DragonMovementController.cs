using Fusion;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonMovementController : CharacterMovementController
    {
        [Networked] public NetworkObject playerNO { set; get; }

        private PlayerMovementController _playerMovement;

        public PlayerMovementController playerController
        {
            get
            {
                if(_playerMovement == null)
                {
                    _playerMovement = playerNO.GetComponent<PlayerMovementController>();
                }

                return _playerMovement;
            }
        }

        public override void Spawned()
        {
            base.Spawned();
            if (HasStateAuthority)
            {
                SwitchTo<DragonIdleMovement>();
            }
        }
    }
}