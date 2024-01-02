using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonAttack
    {
        private string drakaris_name = "Drakaris_Single";
        private int attack = Animator.StringToHash("Attack");
        private Transform drakaris;

        private Animator animator;
        private Transform transform;

        public DragonAttack(Animator animator, Transform transform)
        {
            this.animator = animator;
            this.transform = transform;
            drakaris = Utility.RecursiveFind(transform, drakaris_name);
            Vector3 drakarisScale = new Vector3(0.0f, 1.0f, 0.0f);
            drakaris.transform.localScale = drakarisScale;
        }

        public bool isPlayingAnimation = false;
        
        public void StartAnimation()
        {
            isPlayingAnimation = true;
            animator.SetBool(attack, true);
            drakaris.gameObject.SetActive(true);
        }
        
        public void Update()
        {
            float attackProgress = animator.GetFloat("AttackProgress");
            
            Debug.Log("Attacking:" + attackProgress);

            // Map from 0.0->1.0 to 0.0->1.0->0.0
            float fireProgress = Mathf.Sin(attackProgress * Mathf.PI); // 0->1->0;
            drakaris.transform.localScale = Vector3.one * fireProgress;

            if (attackProgress > 0.9)
            {
                animator.SetBool(attack, false);
                drakaris.gameObject.SetActive(false);
                isPlayingAnimation = false;
            }
        }
    }
}