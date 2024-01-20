using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject.FightSystem
{
    public class PlayerEnemy : Enemy
    {
        [SerializeField] private AudioClip hitAC;

        private AudioSource _ac;
        
        private Animator _animator;
        private int isHitVar = Animator.StringToHash("Hit");
        private int hitProgressVar = Animator.StringToHash("HitProgress");
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _ac = gameObject.AddComponent<AudioSource>();
            _ac.loop = false;
        }
        
        public override string GetName()
        {
            return Runner.GetPlayerUserId(_no.InputAuthority);
        }

        protected override void DoDamage(PlayerRef doneBy)
        {
            if (HasStateAuthority)
            {
                _animator.SetBool(isHitVar, true);

                int deductValue = -3;

                int currentCoinCount = Bonus.Instance.GetCoinCount(_no.StateAuthority);
                if (currentCoinCount + deductValue > 0.0f)
                {
                    Bonus.Instance.AddPlayerCoinRpc(doneBy, 1);
                }
                
                Bonus.Instance.AddPlayerCoinRpc(_no.StateAuthority, deductValue);
            }

            _ac.clip = hitAC;
            _ac.Play();
        }

        protected override void DoDie()
        {
        }

        private void Update()
        {
            if (HasStateAuthority)
            {
                float hitProgress = _animator.GetFloat(hitProgressVar);

                if (hitProgress > 0.9f)
                {
                    _animator.SetBool(isHitVar, false);
                }
            }
        }
    }
}