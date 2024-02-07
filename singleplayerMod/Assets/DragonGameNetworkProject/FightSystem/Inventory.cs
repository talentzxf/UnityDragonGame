

using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class Inventory : MonoBehaviour
    {
        private WEAPONTYPE _primaryWeapon;

        public void Equip(WEAPONTYPE weapon)
        {
            _primaryWeapon = weapon;
        }
    }
}