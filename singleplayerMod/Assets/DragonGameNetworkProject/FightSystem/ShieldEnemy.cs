using System;
using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class ShieldEnemy: Enemy
    {
        [SerializeField] private AudioClip breakAC;

        private GameObject audioGameObject;
        private AudioSource _ac;
        private void Awake()
        {
            audioGameObject = new GameObject("AudioGameObject");
            _ac = audioGameObject.AddComponent<AudioSource>();
            _ac.clip = breakAC;
            _ac.loop = false;
        }

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
            var no = other.gameObject.GetComponentInParent<GameObject>();//todo -- Originally it was networkObject, but now it is temporarily changed to GameObject.
            if (no != null )
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
         //   return "Shield";
            return "";
        }
        
        protected override void DoDie()
        {
            _nameTag.Hide();

            audioGameObject.transform.position = gameObject.transform.position;
            _ac.Play();
            
            gameObject.SetActive(false);
        }
    }
}