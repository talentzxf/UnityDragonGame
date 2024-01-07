using DragonGameNetworkProject.FightSystem;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class ShieldNameTag: AbstractNameTag
    {
        private ShieldEnermy _enermy;
        protected override void InitOnSpawn()
        {
            _enermy = GetComponent<ShieldEnermy>();
        }

        protected override bool HideLocalObjectName()
        {
            return false;
        }

        protected override string GetObjectName()
        {
            return _enermy.GetName();
        }

        protected override Vector3 GetTextPosition()
        {
            return transform.position;
        }
    }
}