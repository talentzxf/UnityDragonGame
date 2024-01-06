namespace DragonGameNetworkProject.FightSystem
{
    public class ShieldEnermy: Enermy
    {
        public override string GetName()
        {
            return "Shield";
        }

        protected override void DoDie()
        {
            gameObject.SetActive(false);
        }
    }
}