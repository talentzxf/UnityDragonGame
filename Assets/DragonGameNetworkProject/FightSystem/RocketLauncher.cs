using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class RocketLauncher: NetworkBehaviour
    {
        [SerializeField] private GameObject rocketPrefab;

        private NetworkObject rocketNO;
        
        private bool _launchRocket = false;

        public override void Spawned()
        {
            base.Spawned();
        }

        void Update()
        {
            if (HasInputAuthority)
            {
                if (Input.GetKeyDown(KeyCode.W))
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
                if (lockedTarget != null)
                {
                    rocketNO = Runner.Spawn(rocketPrefab, transform.position, transform.rotation);
                    rocketNO.gameObject.GetComponent<MissleMove>().Target = lockedTarget.transform;
                }
            }

            _launchRocket = false;
        }
    }
}