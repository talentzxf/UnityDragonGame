using DragonGameNetworkProject.FightSystem;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class ShieldNameTag: AbstractNameTag
    {
        private ShieldEnemy _enemy;
        protected override void InitOnSpawn()
        {
            _enemy = GetComponent<ShieldEnemy>();
        }

        protected override bool HideLocalObjectName()
        {
            return false;
        }

        protected override string GetObjectName()
        {
            return _enemy.GetName();
        }

        protected override Vector3 GetTextPosition()
        {
            return transform.position;
        }
    }
}