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

                Bonus.Instance.AddCoinFromToRpc(_no.StateAuthority, deductValue, doneBy, 1);

                CenterPromptText.Instance.ShowCenterPrompt($"You are hit by {Runner.GetPlayerUserId(doneBy)}, lose {deductValue} points!");
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