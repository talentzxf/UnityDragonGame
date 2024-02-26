using Invector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI
{
    [vClassHeader("AI COMPANION")]
    public class vAICompanion :vMonoBehaviour, vIAIComponent
    {

        public vHealthController friend;
        public string friendTag = "Player";
        public float maxFriendDistance;
        public float minFriendDistance;
        public Type ComponentType
        {
            get
            {
                return this.GetType();
            }
        }

        public bool forceFollow;

        internal vControlAI controlAI;
        protected vAICompanionControl controller;
        protected vAIMovementSpeed speed;

        protected void Start()
        {
            controlAI = GetComponent<vControlAI>();
            controlAI.onDead.AddListener(RemoveCompanion);
            if (!friend) FindFriend();
        }

        private void RemoveCompanion(GameObject arg0)
        {
            if(controller != null && controller.aICompanions.Contains(this))
            {
                controller.aICompanions.Remove(this);
            }
        }

        public bool friendIsFar
        {
           get
            {
                return friendDistance > maxFriendDistance;
            }
        }

        public bool friendIsDead
        {
            get
            {
                return friend && friend.isDead;
            }
        }

        public void FindFriend()
        {
            var fGO = FindObjectsOfType<vHealthController>().vToList().Find(p => p.gameObject.CompareTag(friendTag));
            if(fGO)
            {
                friend = fGO;
                controller = friend.GetComponent<vAICompanionControl>();
                if (controller && !controller.aICompanions.Contains(this)) controller.aICompanions.Add(this);
            }           
        }

        public float friendDistance
        {
            get
            {
                return friend ? (friend.transform.position - transform.position).magnitude : 0;
            }            
        }
     
        public void GoToFriend(vAIMovementSpeed speed = vAIMovementSpeed.Walking)
        {
            if (!friend||!controlAI) return;
            this.speed = speed;
            if (friendDistance > minFriendDistance)
            {                
                controlAI.MoveTo(friend.transform.position, friendDistance > minFriendDistance * 2 ? speed : vAIMovementSpeed.Walking);
            }
            else
            {
                controlAI._headtrack.LookAtTarget(friend.transform,timeToExitLookTarget:0.1f);
                controlAI.Stop();
            }
        }
    }
}