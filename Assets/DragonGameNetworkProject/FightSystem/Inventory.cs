using Fusion;

namespace DragonGameNetworkProject.FightSystem
{
    public class Inventory : NetworkBehaviour
    {
        private WEAPONTYPE _primaryWeapon;

        public void Equip(WEAPONTYPE weapon)
        {
            _primaryWeapon = weapon;
        }
    }
}