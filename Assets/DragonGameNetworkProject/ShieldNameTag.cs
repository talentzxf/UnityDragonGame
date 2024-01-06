using UnityEngine;

namespace DragonGameNetworkProject
{
    public class ShieldNameTag: AbstractNameTag
    {
        protected override void InitOnSpawn()
        {
            
        }

        protected override bool HideLocalObjectName()
        {
            return false;
        }

        protected override string GetObjectName()
        {
            return "Shield";
        }

        protected override Vector3 GetTextPosition()
        {
            return transform.position;
        }
    }
}