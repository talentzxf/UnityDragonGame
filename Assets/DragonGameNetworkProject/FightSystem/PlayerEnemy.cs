namespace DragonGameNetworkProject.FightSystem
{
    public class PlayerEnemy : Enemy
    {
        public override string GetName()
        {
            return Runner.GetPlayerUserId(_no.InputAuthority);
        }

        protected override void DoDie()
        {
            
        }
    }
}