using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class RocketLauncher: NetworkBehaviour
    {
        [SerializeField] private GameObject rocketPrefab;

        private NetworkObject rocketNO;
        
        private bool _launchRocket;

        void Update()
        {
            if (HasInputAuthority)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    _launchRocket = true;
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (_launchRocket)
            {
                Enemy lockedTarget = EnemyManager.Instance.GetCurrentLockedTarget();
                if (lockedTarget != null && lockedTarget.gameObject.activeSelf)
                {
                    rocketNO = Runner.Spawn(rocketPrefab, transform.position, transform.rotation);
                    rocketNO.gameObject.GetComponent<MissileMove>().Target = lockedTarget.transform;
                }
            }

            _launchRocket = false;
        }
    }
}