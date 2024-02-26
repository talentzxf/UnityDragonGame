using UnityEngine;

namespace Invector.vCharacterController.AI
{
    using System.Collections.Generic;
    using vEventSystems;

    public partial interface vIControlAI : vIHealthController
    {

        /// <summary>
        /// Used just to Create AI Editor
        /// </summary>
        void CreatePrimaryComponents();

        /// <summary>
        /// Used just to Create AI Editor
        /// </summary>
        void CreateSecondaryComponents();

        /// <summary>
        /// Check if <seealso cref="vIControlAI"/> has a <seealso cref=" vIAIComponent"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasComponent<T>() where T : vIAIComponent;

        /// <summary>
        /// Get Specific <seealso cref="vIAIComponent"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetAIComponent<T>() where T : vIAIComponent;

        Vector3 selfStartPosition { get; set; }
        Vector3 targetDestination { get; }
        Collider selfCollider { get; }
        Animator animator { get; }
        vAnimatorStateInfos animatorStateInfos { get; }
        vWaypointArea waypointArea { get; set; }
        vAIReceivedDamegeInfo receivedDamage { get; }
        vWaypoint targetWaypoint { get; }
        List<vWaypoint> visitedWaypoints { get; set; }

        Vector3 lastTargetPosition { get; }
        bool ragdolled { get; }
        bool isInDestination { get; }
        bool isMoving { get; }
        bool isStrafing { get; }
        bool isRolling { get; }
        bool isCrouching { get; set; }
        bool targetInLineOfSight { get; }
        vAISightMethod SightMethod { get; set; }
        vAIUpdateQuality UpdatePathQuality { get; set; }
        vAIUpdateQuality FindTargetUpdateQuality { get; set; }
        vAIUpdateQuality CanseeTargetUpdateQuality { get; set; }
        vAIMovementSpeed movementSpeed { get; }
        float targetDistance { get; }
        float changeWaypointDistance { get; }
        Vector3 desiredVelocity { get; }
        float remainingDistance { get; }
        float stopingDistance { get; set; }
        float minDistanceToDetect { get; set; }
        float maxDistanceToDetect { get; set; }
        float fieldOfView { get; set; }
        bool selfStartingPoint { get; }
        bool customStartPoint { get; }

        Vector3 customStartPosition { get; }
        void SetDetectionLayer(LayerMask mask);
        void SetDetectionTags(List<string> tags);
        void SetObstaclesLayer(LayerMask mask);
        void SetLineOfSight(float fov = -1, float minDistToDetect = -1, float maxDistToDetect = -1, float lostTargetDistance = -1);
        void NextWayPoint();

        /// <summary>
        /// Move AI to a position in World Space
        /// </summary>
        /// <param name="destination">world space position</param>
        /// <param name="speed">movement speed</param>
        void MoveTo(Vector3 destination, vAIMovementSpeed speed = vAIMovementSpeed.Walking);
        /// <summary>
        /// Move AI to a position in World Space and rotate to a custom direction
        /// </summary>
        /// <param name="destination">world space position</param>
        /// <param name="forwardDirection">target rotation direction</param>
        /// <param name="speed">>movement speed</param>
        void StrafeMoveTo(Vector3 destination, Vector3 forwardDirection, vAIMovementSpeed speed = vAIMovementSpeed.Walking);

        /// <summary>
        /// Move AI to a position in World Space with out update the target rotation direction of the AI
        /// </summary>
        /// <param name="destination">world space position</param>      
        /// <param name="speed">>movement speed</param>
        void StrafeMoveTo(Vector3 destination, vAIMovementSpeed speed = vAIMovementSpeed.Walking);

        void RotateTo(Vector3 direction);
        void RollTo(Vector3 direction);
        void SetCurrentTarget(Transform target);
        void SetCurrentTarget(Transform target, bool overrideCanseeTarget);
        void SetCurrentTarget(vAITarget target);
        void SetCurrentTarget(vAITarget target, bool overrideCanseeTarget);
        void RemoveCurrentTarget();
        /// <summary>
        /// Current target
        /// </summary>
        vAITarget currentTarget { get; }

        /// <summary>
        /// Secundary targets storage
        /// </summary>
        List<vAITarget> secundaryTargets { get; set; }
        /// <summary>
        /// Return a list of targets resulted of <seealso cref="FindTarget"/> method
        /// </summary>
        /// <returns></returns>
        List<vAITarget> GetTargetsInRange();
        /// <summary>
        /// Find target using Detection settings
        /// </summary>
        void FindTarget();
        /// <summary>
        /// Find target with ignoring obstacles option and set to <see cref="currentTarget"/>
        /// </summary>
        /// <param name="checkForObstacles"></param>
        void FindTarget(bool checkForObstacles);
        /// <summary>
        /// Try get a target 
        /// <remarks>Needs to call <see cref="FindTarget"/> method to detect all target in range.The result is the most closest target</remarks>
        /// </summary>       
        /// <param name="target"></param>
        /// <returns></returns>
        bool TryGetTarget(out vAITarget target);
        /// <summary>
        /// Try get a target with specific tag 
        /// <remarks>Needs to call <see cref="FindTarget"/> method to detect all target in range.The result is the most closest target</remarks>
        /// </summary>        
        /// <param name="tag">possible target tag</param>
        /// <param name="target"></param>
        /// <returns></returns>
        bool TryGetTarget(string tag, out vAITarget target);
        /// <summary>
        /// Try get a target with specific tags
        /// </summary>
        /// <remarks>Needs to call <see cref="FindTarget"/> method to detect all target in range.The result is the most closest target</remarks>
        /// <param name="m_detectTags">list of possible target tags</param>
        /// <param name="target"></param>
        /// <returns></returns>
        bool TryGetTarget(List<string> m_detectTags, out vAITarget target);

        /// <summary>
        /// Find target with specific detection settings
        /// </summary>
        /// <remarks>Needs to call <see cref="FindTarget"/> method to detect all target in range.The result is the most closest target</remarks>
        /// <param name="m_detectTags">list of possible target tags</param>
        /// <param name="m_detectLayer">layer of possible target</param>
        /// <param name="checkForObstables"></param>
        void FindSpecificTarget(List<string> m_detectTags, LayerMask m_detectLayer, bool checkForObstables = true);

        void LookAround();
        void LookTo(Vector3 point, float stayLookTime = 1f, float offsetLookHeight = -1);
        void LookToTarget(Transform target, float stayLookTime = 1f, float offsetLookHeight = -1);
        void Stop();
        void ForceUpdatePath(float timeInUpdate = 1f);

        /// <summary>
        /// Check if AI is Trigger With some collider with specific tag
        /// </summary>
        /// <param name="targ"></param>
        /// <returns></returns>
        bool IsInTriggerWithTag(string tag);
        /// <summary>
        /// Check if AI is Trigger With some collider with specific name
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        bool IsInTriggerWithName(string name);

        /// <summary>
        /// Check if AI is Trigger With some collider with specific name
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool IsInTriggerWithTag(string tag, out Collider result);
        /// <summary>
        /// Check if AI is Trigger With some collider with specific tag
        /// </summary>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool IsInTriggerWithName(string name, out Collider result);
    }

    public partial interface vIControlAICombat : vIControlAI
    {
        int strafeCombatSide { get; }
        float minDistanceOfTheTarget { get; }
        float combatRange { get; }
        bool isInCombat { get; set; }
        bool strafeCombatMovement { get; }

        int attackCount { get; set; }
        float attackDistance { get; }
        bool isAttacking { get; }
        bool canAttack { get; }
        void InitAttackTime();
        void ResetAttackTime();
        void Attack(bool strongAttack = false, int attackID = -1, bool forceCanAttack = false);

        bool isBlocking { get; }
        bool canBlockInCombat { get; }
        void ResetBlockTime();
        void Blocking();

        void AimTo(Vector3 point, float stayLookTime = 1f, object sender = null);
        void AimToTarget(float stayLookTime = 1f, object sender = null);

        bool isAiming { get; }
        bool isArmed { get; }
    }

    public partial interface vIControlAIMelee : vIControlAICombat
    {
        vMelee.vMeleeManager MeleeManager { get; set; }
        void SetMeleeHitTags(List<string> tags);
    }

    public partial interface vIControlAIShooter : vIControlAICombat
    {
        vAIShooterManager shooterManager { get; set; }
        void SetShooterHitLayer(LayerMask mask);
        /// <summary>
        /// Check if Aim is aligned to the target position setted from method <see cref="vIControlAICombat.AimTo(Vector3, float, object)"/>
        /// </summary>
        bool IsInShotAngle { get; }
    }
}