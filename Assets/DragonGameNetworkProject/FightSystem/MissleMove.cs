using System.Collections;
using System.Threading;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class MissleMove : NetworkBehaviour
    {
        [SerializeField] private GameObject _explodePrefab;

        [SerializeField] private Transform _target;
        [SerializeField] private float initSpeed = 10.0f;

        [SerializeField] private float rotationSpeed = 100.0f;
        [SerializeField] private bool equipped = false;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        private Rigidbody rb;

        private Collider _collider;

        public override void Spawned()
        {
            base.Spawned();
            rb = GetComponent<Rigidbody>();

            equipped = false;
            _collider = GetComponent<Collider>();

            // _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && HasStateAuthority)
            {
                var inventory = other.GetComponentInParent<Inventory>();
                inventory.Equip(WEAPONTYPE.ROCKET);
            }
        }

        [Networked] private bool exploded { set; get; }
        [Networked] private Vector3 explosionPoint { set; get; }
        [Networked] private Vector3 explosionNormal { set; get; }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                var otherColliderNO = contact.otherCollider.GetComponentInParent<NetworkObject>();
                var enemy = contact.otherCollider.GetComponentInParent<Enemy>();

                if (enemy is PlayerEnemy && otherColliderNO.HasInputAuthority) // Can't hit myself.
                    continue;

                {
                    if (HasStateAuthority)
                    {
                        explosionPoint = contact.point;
                        explosionNormal = contact.normal;

                        exploded = true;

                        if (enemy)
                            enemy.DoDamageRpc(1000.0f);
                    }
                }
            }
        }

        void AdjustAim(float delta)
        {
            if (Target == null)
                return;

            Vector3 heading = Target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(heading);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * delta));

            rb.velocity = transform.forward * initSpeed;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, Target.transform.position);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + rb.velocity * 10.0f);
            }
        }
#endif

        private void Update()
        {
            if (Runner == null || Runner.State != NetworkRunner.States.Running)
                return;

            if (exploded)
            {
                var explosionPrefab = Instantiate(_explodePrefab);
                explosionPrefab.transform.position = explosionPoint;
                explosionPrefab.transform.rotation = Quaternion.LookRotation(explosionNormal);
                
                gameObject.SetActive(false);
                // StartCoroutine(DespawnMySelf());
            }
        }

        // TODO: Run GC in another function.
        private IEnumerator DespawnMySelf()
        {
            yield return new WaitForSeconds(1); // After 1 second to ensure all clients explosion effect played.
            Runner.Despawn(GetComponent<NetworkObject>()); // Only State auth can despawn the rocket.
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                AdjustAim(Runner.DeltaTime);
            }
        }
    }
}