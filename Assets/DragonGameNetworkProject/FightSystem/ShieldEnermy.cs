namespace DragonGameNetworkProject.FightSystem
{
    public class ShieldEnermy: Enermy
    {
        protected override string GetName()
        {
            return "Shield";
        }

        protected override void DoDie()
        {
            gameObject.SetActive(false);
        }
    }
}