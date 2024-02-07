
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class PlayerNameTag : AbstractNameTag
    {
        private Transform avatarTransform;
        private float objectHeight = 1.6f;

        private CharacterController _cc;


        protected override void InitOnSpawn()
        {
            _cc = gameObject.GetComponentInChildren<CharacterController>();
            if (_cc != null)
            {
                avatarTransform = _cc.transform;
                objectHeight = _cc.height;
            }
        }

        protected override bool HideLocalObjectName()
        {
            return true;
        }

        protected override string GetObjectName()
        {
            // var no = GetComponent<NetworkObject>();
            // return Runner.GetPlayerUserId(no.InputAuthority);
            return null;
        }

        public override Vector3 GetTextPosition()
        {
            return avatarTransform.position + (objectHeight * 1.1f) * avatarTransform.up;
        }
    }
}