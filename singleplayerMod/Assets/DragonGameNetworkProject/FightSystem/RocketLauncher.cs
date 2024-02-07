using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class RocketLauncher: MonoBehaviour
    {
        [SerializeField] private GameObject rocketPrefab;

        private GameObject rocketNO;
        
        private bool _launchRocket;

        void Update()
        {
            //if (HasInputAuthority)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    _launchRocket = true;
                }
            }
        }

        public  void FixedUpdate()
        {
            if (_launchRocket)
            {
                Enemy lockedTarget = EnemyManager.Instance.GetCurrentLockedTarget();
                if (lockedTarget != null && lockedTarget.gameObject.activeSelf)
                {
                    rocketNO = Instantiate(rocketPrefab, transform.position, transform.rotation);
                    rocketNO.gameObject.GetComponent<MissileMove>().Target = lockedTarget.transform;
                }
            }

            _launchRocket = false;
        }
    }
}