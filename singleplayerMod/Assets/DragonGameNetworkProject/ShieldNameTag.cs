using DragonGameNetworkProject.FightSystem;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class ShieldNameTag: AbstractNameTag
    {
        private ShieldEnemy _enemy;

        private SphereCollider _sphereCollider;
        protected override void InitOnSpawn()
        {
            _enemy = GetComponent<ShieldEnemy>();
            _sphereCollider = GetComponent<SphereCollider>();
        }

        protected override bool HideLocalObjectName()
        {
            return false;
        }

        protected override string GetObjectName()
        {
            return _enemy.GetName();
        }

        public override Vector3 GetTextPosition()
        {
            if(_sphereCollider == null)
                return transform.position;

            return transform.position + 2.0f * _sphereCollider.radius * transform.localScale.y * transform.up;
        }
    }
}