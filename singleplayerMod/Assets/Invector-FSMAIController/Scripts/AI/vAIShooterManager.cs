
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Invector.vCharacterController.AI
{
    using vShooter;
    [vClassHeader("AI SHOOTER MANAGER", "Make sure to set the Damage Layers to 'Default' and 'BodyPart', or any other layer you need to inflict damage.")]
    public class vAIShooterManager : vMonoBehaviour
    {
        #region variables

        [System.Serializable]
        public class OnReloadWeapon : UnityEngine.Events.UnityEvent<vShooterWeapon> { }

        [vEditorToolbar("Aim")]     
        [Tooltip("min distance to aim")]
        public float minDistanceToAim = 1;
        public float checkAimRadius = 0.1f;
        [Tooltip("smooth of the right hand when correcting the aim")]
        public float smoothHandRotation = 30f;
        [Tooltip("Limit the maxAngle for the right hand to correct the aim")]
        public float maxHandAngle = 60f;  
        [Tooltip("Check this to syinc the weapon aim to the camera aim")]
        public bool raycastAimTarget = true;
        [Tooltip("Layer to aim")]
        public LayerMask damageLayer = 1 << 0;
        [Tooltip("Tags to the Aim ignore - tag this gameObject to avoid shot on yourself")]
        public List<string> ignoreTags;

        [vEditorToolbar("IK Adjust")]
        [Tooltip("Check this to use IK on the left hand")]
        public bool useLeftIK = true, useRightIK = true;  
        public vWeaponIKAdjustList weaponIKAdjustList;

        [vEditorToolbar("Weapons")]
        public vShooterWeapon rWeapon, lWeapon;
        [HideInInspector]
        public OnReloadWeapon onReloadWeapon;

      
        private Animator animator;
        private int totalAmmo;
        private int secundaryTotalAmmo;      
        protected vWeaponIKAdjust currentWeaponIKAdjust;
        #endregion

        void Start()
        {
            animator = GetComponent<Animator>();

            if (animator)
            {
                var _rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                var _lefttHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);

                var weaponR = _rightHand.GetComponentInChildren<vShooterWeapon>(true);
                var weaponL = _lefttHand.GetComponentInChildren<vShooterWeapon>(true);
                if (weaponR != null)
                    SetRightWeapon(weaponR.gameObject);
                if (weaponL != null)
                    SetLeftWeapon(weaponL.gameObject);
            }

            if (!ignoreTags.Contains(gameObject.tag))
                ignoreTags.Add(gameObject.tag);
        }
        public void SetDamageLayer(LayerMask mask)
        {
            damageLayer = mask;
            if (CurrentWeapon) CurrentWeapon.hitLayer = mask;
        }
        public void SetLeftWeapon(GameObject weapon)
        {
            if (weapon != null)
            {
                var w = weapon.GetComponentInChildren<vShooterWeapon>(true);
                lWeapon = w;
                if (lWeapon)
                {
                    lWeapon.ignoreTags = ignoreTags;
                    lWeapon.hitLayer = damageLayer;
                    lWeapon.root = transform;
                    lWeapon.isSecundaryWeapon = false;
                    lWeapon.onDestroy.AddListener(OnDestroyWeapon);
                    if (lWeapon.dontUseReload) ReloadWeaponAuto(lWeapon);                   
                    UpdateWeaponIK();
                }
            }
        }

        public void SetRightWeapon(GameObject weapon)
        {
            if (weapon != null)
            {
                var w = weapon.GetComponentInChildren<vShooterWeapon>(true);
                rWeapon = w;
                if (rWeapon)
                {
                    rWeapon.ignoreTags = ignoreTags;
                    rWeapon.hitLayer = damageLayer;
                    rWeapon.root = transform;
                    rWeapon.isSecundaryWeapon = false;
                    rWeapon.onDestroy.AddListener(OnDestroyWeapon);
                    if (rWeapon.dontUseReload) ReloadWeaponAuto(rWeapon);                   
                    UpdateWeaponIK();
                }
            }
        }


        public virtual void SetIKAdjustList(vWeaponIKAdjustList weaponIKAdjustList)
        {
            this.weaponIKAdjustList = weaponIKAdjustList;
            UpdateWeaponIK();
        }

        public virtual vWeaponIKAdjust CurrentWeaponIK
        {
            get
            {
                return currentWeaponIKAdjust;
            }
        }
        public virtual void UpdateWeaponIK()
        {
            if (weaponIKAdjustList && CurrentWeapon)
            {
                currentWeaponIKAdjust = weaponIKAdjustList.GetWeaponIK(CurrentWeapon.weaponCategory);                
            }
               
        }
       
        public void OnDestroyWeapon(GameObject otherGameObject)
        {
           
        }

        public int GetMoveSetID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.moveSetID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.moveSetID;
            return id;
        }

        public int GetUpperBodyID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.upperBodyID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.upperBodyID;
            return id;
        }

        public int GetShotID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.shotID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.shotID;
            return id;
        }

        public int GetAttackID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.shotID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.shotID;
            return id;
        }

        public int GetEquipID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.equipID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.equipID;
            return id;
        }

        public int GetReloadID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeSelf) id = (int)rWeapon.reloadID;
            else if (lWeapon && lWeapon.gameObject.activeSelf) id = (int)lWeapon.reloadID;
            return id;
        }

        public bool isShooting
        {
            get { return CurrentWeapon && !CurrentWeapon.CanDoShot; }
        }

        public void ReloadWeapon()
        {
            var weapon = CurrentWeapon;

            if (!weapon || !weapon.gameObject.activeSelf) return;

          
            if (!((weapon.ammoCount >= weapon.clipSize)) && !weapon.dontUseReload)
            {
               
                onReloadWeapon.Invoke(weapon);
                var needAmmo = weapon.clipSize - weapon.ammoCount;

                weapon.AddAmmo(needAmmo);

                if (animator)
                {
                    animator.SetInteger("ReloadID", GetReloadID());
                    animator.SetTrigger("Reload");
                }              
                weapon.ReloadEffect();             
            }
           
        }

        protected void ReloadWeaponAuto(vShooterWeapon weapon)
        {
            if (!weapon || !weapon.gameObject.activeSelf) return;

            if (!((weapon.ammoCount >= weapon.clipSize)))
            {
                var needAmmo = weapon.clipSize - weapon.ammoCount;
                weapon.AddAmmo(needAmmo);
            }
        }

        public virtual bool weaponHasAmmo
        {
            get
            {
                if (!CurrentWeapon) return false;
                return CurrentWeapon.ammoCount > 0;
            }
        }

        public virtual vShooterWeapon CurrentWeapon
        {
            get { return rWeapon && rWeapon.gameObject.activeSelf ? rWeapon : lWeapon && lWeapon.gameObject.activeSelf ? lWeapon : null; }
        }

        public bool IsLeftWeapon
        {
            get
            {
                var isLeftWp = (rWeapon == null) ?
                    (lWeapon) : false;
                return isLeftWp;
            }
        }

        public virtual void Shoot(Vector3 aimPosition)
        { 
            var weapon = CurrentWeapon;
            if (!weapon || !weapon.gameObject.activeSelf) return;

          
            var targetWeapon = weapon;
            if (targetWeapon.dontUseReload) ReloadWeaponAuto(targetWeapon);
            var applyRecoil = false;
            targetWeapon.Shoot(aimPosition, transform,(bool sucessful)=> { applyRecoil = sucessful; });
            if(applyRecoil)
                StartCoroutine(Recoil());
            if (targetWeapon.dontUseReload) ReloadWeaponAuto(targetWeapon);           
        }

        IEnumerator Recoil()
        {
            yield return new WaitForSeconds(0.02f);
            if (animator) animator.SetTrigger("Shoot");
        }    
    }
}
