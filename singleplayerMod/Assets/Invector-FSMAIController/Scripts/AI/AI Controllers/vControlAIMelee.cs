using Invector.vEventSystems;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI
{
    [vClassHeader(" AI MELEE CONTROLLER", iconName = "AI-icon")]
    public partial class vControlAIMelee : vControlAICombat, vIControlAIMelee, vIMeleeFighter
    {
        public vMelee.vMeleeManager MeleeManager { get; set; }
        public bool isEquipping { get; protected set; }
        protected int _moveSetID;
        protected int _attackID;
        protected int _defenseID;
        protected int _recoilID;

        protected override void Start()
        {
            base.Start();
            MeleeManager = GetComponent<vMelee.vMeleeManager>();
        }

        public override void CreateSecondaryComponents()
        {
            base.CreateSecondaryComponents();
            if (GetComponent<vMelee.vMeleeManager>() == null) gameObject.AddComponent<vMelee.vMeleeManager>();
        }

        protected virtual int moveSetID
        {
            get
            {
                return _moveSetID;
            }
            set
            {
                if (value != _moveSetID || animator.GetFloat("MoveSet_ID") != value)
                {
                    _moveSetID = value;
                    animator.SetFloat("MoveSet_ID", (float)_moveSetID, 0.25f, Time.deltaTime);
                }
            }
        }

        protected virtual int attackID
        {
            get
            {
                return _attackID;
            }
            set
            {
                if (value != _attackID)
                {
                    _attackID = value;
                    animator.SetInteger("AttackID", _attackID);
                }
            }
        }

        protected virtual int defenseID
        {
            get
            {
                return _defenseID;
            }
            set
            {
                if (value != _defenseID)
                {
                    _defenseID = value;
                    animator.SetInteger("DefenseID", _defenseID);
                }
            }
        }

        public override bool isArmed { get { return MeleeManager != null ? MeleeManager.rightWeapon != null : false; } }

        public virtual vICharacter character { get { return this; } }

        public virtual void SetMeleeHitTags(List<string> tags)
        {
            if (MeleeManager) MeleeManager.hitProperties.hitDamageTags = tags;
        }

        public override void Attack(bool strongAttack = false, int _newAttackID = -1, bool forceCanAttack = false)
        {
            if (MeleeManager && _newAttackID != -1)
            {
                attackID = _newAttackID;
            }
            else
            {
                attackID = MeleeManager.GetAttackID();
            }

            base.Attack(strongAttack, _newAttackID, forceCanAttack);
        }

        protected override void UpdateCombatAnimator()
        {
            base.UpdateCombatAnimator();
            isEquipping = IsAnimatorTag("IsEquipping");
            if (MeleeManager)
            {
                moveSetID = MeleeManager.GetMoveSetID();
                defenseID = MeleeManager.GetDefenseID();
            }
        }

        protected override void TryBlockAttack(vDamage damage)
        {
            base.TryBlockAttack(damage);
            if (MeleeManager && damage.sender)
            {
                if (isBlocking && MeleeManager.CanBlockAttack(damage.sender.position))
                {
                    var fighter = damage.sender.GetComponent<vIMeleeFighter>();
                    var damageReduction = MeleeManager.GetDefenseRate();
                    if (damageReduction > 0)
                        damage.ReduceDamage(damageReduction);
                    if (fighter != null && MeleeManager.CanBreakAttack())
                        fighter.OnRecoil(MeleeManager.GetDefenseRecoilID());
                    MeleeManager.OnDefense();
                }
                else damage.hitReaction = true;
            }
        }

        protected virtual void TryApplyRecoil(vIMeleeFighter fighter)
        {
            if (MeleeManager && fighter != null)
            {
                if (isBlocking && MeleeManager.CanBlockAttack(fighter.transform.position))
                {
                    if (MeleeManager.CanBreakAttack())
                        fighter.OnRecoil(MeleeManager.GetDefenseRecoilID());
                }
            }
        }

        public virtual void BreakAttack(int breakAtkID)
        {
            ResetAttackTime();
            ResetAttackTriggers();
            OnRecoil(breakAtkID);
        }

        public virtual void OnRecoil(int recoilID)
        {
            if (animator != null && animator.enabled && !isRolling)
            {
                animator.SetInteger("RecoilID", recoilID);
                animator.SetTrigger("TriggerRecoil");
                animator.SetTrigger("ResetState");
                ResetAttackTriggers();
            }
        }

        public virtual void OnReceiveAttack(vDamage damage, vIMeleeFighter attacker)
        {
            TakeDamage(damage);
            TryApplyRecoil(attacker);
        }
    }
}
