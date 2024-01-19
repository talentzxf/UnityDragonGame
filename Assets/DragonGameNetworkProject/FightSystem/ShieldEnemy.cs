using System;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class ShieldEnemy: Enemy
    {
        private float reflectForceMag = 15.0f;
        private void OnCollisionEnter(Collision other)
        {
            AvoidCollision(other);
        }

        private void OnCollisionStay(Collision other)
        {
            AvoidCollision(other);
        }

        private void AvoidCollision(Collision other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            var no = other.gameObject.GetComponentInParent<NetworkObject>();
            if (no != null && no.HasStateAuthority)
            {
                var rb = other.gameObject.GetComponentInParent<Rigidbody>();

                Vector3 impulseForce = (other.transform.position - transform.position).normalized * reflectForceMag;

                // Vector3 forceDir = Vector3.Cross(bumpDir, other.transform.up);
                //
                // forceDir += bumpDir;

                // Vector3 impulseForce = forceDir.normalized * reflectForceMag;
                Debug.Log($"Collided with shield, add a force.{impulseForce}");

                rb.AddForce(impulseForce * Time.deltaTime, ForceMode.Impulse);
            }
        }


        public override string GetName()
        {
            return "Shield";
        }
        
        protected override void DoDie()
        {
            gameObject.SetActive(false);
            _nameTag.Hide();
        }
    }
}