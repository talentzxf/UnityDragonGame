using System.Collections;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace DragonGameNetworkProject.FightSystem
{
    public class MissileMove : NetworkBehaviour
    {
        [SerializeField] private GameObject _explodePrefab;

        [SerializeField] private Transform _target;
        [SerializeField] private float initSpeed = 10.0f;

        [SerializeField] private float rotationSpeed = 100.0f;

        [SerializeField] private AudioClip launchAC;
        [SerializeField] private float missileVolume = 0.5f;
        [SerializeField] private AudioClip rocketExplode;

        private AudioSource _audioSource;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        private Rigidbody rb;

        private Collider _collider;

        private NetworkObject _no;

        private GameObject audioGameObject;
        public override void Spawned()
        {
            base.Spawned();
            rb = GetComponent<Rigidbody>();

            _no = GetComponent<NetworkObject>();
            
            _collider = GetComponent<Collider>();

            audioGameObject = new GameObject("AudioGameObject");
            audioGameObject.transform.position = transform.position;
            
            _audioSource = audioGameObject.AddComponent<AudioSource>();
            _audioSource.volume = missileVolume;
            _audioSource.clip = launchAC;
            _audioSource.loop = false;
            _audioSource.Play();

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
                        {
                            if (enemy is PlayerEnemy)
                            {
                                CenterPromptText.Instance.ShowCenterPrompt($"Your missile hit:{enemy.GetName()}, you might get 1 point!");
                            }
                            
                            enemy.DoDamageRpc(_no.StateAuthority, 1000.0f);
                        }
                    }
                }
            }
        }

        void AdjustAim(float delta)
        {
            if (Target == null)
            {
                return;
            }

            if (!Target.gameObject.activeSelf)
            {
                explosionPoint = transform.position;
                explosionNormal = Vector3.up;

                exploded = true;
                return;
            }


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

        private bool explodeACPlayed = false;
        
        private void Update()
        {
            if (Runner == null || Runner.State != NetworkRunner.States.Running)
                return;

            if (HasStateAuthority)
            {
                AdjustAim(Time.deltaTime);
            }
            
            if (exploded)
            {
                var explosionPrefab = Instantiate(_explodePrefab);
                explosionPrefab.transform.position = explosionPoint;
                explosionPrefab.transform.rotation = Quaternion.LookRotation(explosionNormal);

                if (!explodeACPlayed)
                {
                    audioGameObject.transform.position = transform.position;
                    _audioSource.clip = rocketExplode;
                    _audioSource.Play();
                    explodeACPlayed = true;
                }
                
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

        // public override void FixedUpdateNetwork()
        // {
        //     if (HasStateAuthority)
        //     {
        //         AdjustAim(Runner.DeltaTime);
        //     }
        // }
    }
}