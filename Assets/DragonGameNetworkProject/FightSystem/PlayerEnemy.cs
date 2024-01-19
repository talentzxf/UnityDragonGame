using Fusion;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DragonGameNetworkProject.FightSystem
{
    public class PlayerEnemy : Enemy
    {
        private Animator _animator;
        private NetworkObject _no;
        private int isHitVar = Animator.StringToHash("Hit");
        private int hitProgressVar = Animator.StringToHash("HitProgress");
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _no = GetComponentInParent<NetworkObject>();
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
                Bonus.Instance.AddPlayerCoinRpc(_no.StateAuthority, -3);
                Bonus.Instance.AddPlayerCoinRpc(doneBy, 1);
            }
        }

        protected override void DoDie()
        {
        }

        private void Update()
        {
            if (HasStateAuthority)
            {
                float hitProgress = _animator.GetFloat(hitProgressVar);

                if (hitProgress > 0.0f)
                {
                    _animator.SetBool(isHitVar, false);
                }
            }
        }
    }
}