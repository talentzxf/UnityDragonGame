using Unity.VisualScripting;

namespace DragonGameNetworkProject.FightSystem
{
    public class ShieldEnemy: Enemy
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