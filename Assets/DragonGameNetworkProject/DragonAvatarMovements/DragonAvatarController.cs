using System;
using Fusion;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarController : CharacterMovementController
    {
        [Networked] private Color bodyColor { set; get; }
        [Networked] private Color hairColor { set; get; }

        public void SetBodyColor(Color bodyColor)
        {
            this.bodyColor = bodyColor;
        }

        public void SetHairColor(Color hairColor)
        {
            this.hairColor = hairColor;
        }

        private SkinnedMeshRenderer smr;
        private void Awake()
        {
            smr = avatarGO.GetComponentInChildren<SkinnedMeshRenderer>();
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                if (collider is not CharacterController)
                {
                    collider.gameObject.tag = "Player"; // Setup tags.
                    collider.isTrigger = true;
                }
            }
        }

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        private void Update()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(bodyColor):
                        smr.materials[3].color = bodyColor;
                        break;
                    case nameof(hairColor):
                        smr.materials[2].color = hairColor;
                        break;
                }
            }
        }
    }
}