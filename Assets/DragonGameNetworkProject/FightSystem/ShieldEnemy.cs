using System;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class ShieldEnemy: Enemy
    {
        [SerializeField] private float reflectForceMag = 1.0f;
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

                Vector3 bumpDir = (other.transform.position - transform.position).normalized;

                Vector3 forceDir = Vector3.Cross(bumpDir, other.transform.up);
                    
                Debug.Log("Collided with shield, add a force.");

                rb.AddForce(forceDir.normalized * reflectForceMag, ForceMode.Impulse);
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