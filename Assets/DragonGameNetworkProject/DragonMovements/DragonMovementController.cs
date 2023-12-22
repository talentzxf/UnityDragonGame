namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonMovementController : CharacterMovementController
    {
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