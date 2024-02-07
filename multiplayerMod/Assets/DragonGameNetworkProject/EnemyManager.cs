using System;
using System.Collections.Generic;
using DragonGameNetworkProject.DragonAvatarMovements;
using DragonGameNetworkProject.FightSystem;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    class EnemyInfo : IComparable<EnemyInfo>
    {
        private readonly Transform _enemyTransform;
        private Transform _playerTransform;
        private float _distance;
        private bool _isVisibleOnScreen;
        private bool _isInDistance;
        private float _maxLockDistance;

        private Transform PlayerTransform {
            get
            {
                if (_playerTransform == null)
                {
                    _playerTransform = DragonAvatarController.LocalController.avatarGO.transform;
                }

                return _playerTransform;
            }
        }
        public Enemy Enemy { get; }

        public bool IsLockable => _isVisibleOnScreen && _isInDistance;
        public float Distance => _distance;

        public EnemyInfo(Enemy enemy, float maxLockDistance)
        {
            Enemy = enemy;
            _enemyTransform = enemy.transform;
            _maxLockDistance = maxLockDistance;
        }

        public void Update()
        {
            if (_enemyTransform == null)
                return;
            
            Vector3 enemyPosition = _enemyTransform.position;
            _distance = Vector3.Distance(enemyPosition, PlayerTransform.position);
            _isVisibleOnScreen = Utility.IsVisibleOnScreen(Camera.main, enemyPosition, out _);

            if (!_isVisibleOnScreen)
            {
                _distance = float.MaxValue;
            }

            _isInDistance = _distance <= _maxLockDistance;
        }
        
        public int CompareTo(EnemyInfo other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return _distance.CompareTo(other._distance);
        }
    }
    
    public class EnemyManager: MonoBehaviour
    {
        private static EnemyManager _instance;

        private List<EnemyInfo> _enemies = new();

        private const float MaxLockDistance = 100.0f;

        [SerializeField] private AudioClip lockAC;

        private AudioSource _audioSource;

        public static EnemyManager Instance => _instance;

        private CharacterMovementController _localPlayer;

        public void RegisterEnemy(Enemy enemy)
        {
            _enemies.Add(new EnemyInfo(enemy, MaxLockDistance));
        }

        private EnemyInfo _lockedEnemy;

        public Enemy GetCurrentLockedTarget()
        {
            if (_lockedEnemy == null)
                return null;
            
            _lockedEnemy.Update();
            if (!_lockedEnemy.IsLockable)
                return null;

            return _lockedEnemy.Enemy;
        }

        private void Update()
        {
            if (_lockedEnemy != null)
            {
                _lockedEnemy.Update();
                if (_lockedEnemy is {IsLockable: false}) // Use C# 9 pattern match grammar.
                {
                    // Reset to normal status.
                    _lockedEnemy.Enemy.Unlock();
                    _lockedEnemy = null;
                }
                else
                {
                    _lockedEnemy.Enemy.ShowDistance(_lockedEnemy.Distance);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Q)) // Every time press tab to lock the nearest visible enermy.
            {
                _enemies.RemoveAll(item =>
                {
                    if (item.Enemy is PlayerEnemy && (item.Enemy.GetComponent<NetworkObject>()?.HasInputAuthority ?? false))
                    {
                        return true;
                    }
                        
                    return item.Enemy == null ||
                           !item.Enemy.gameObject.activeSelf || !item.Enemy.enabled;
                });
                
                if (_enemies.Count == 0)
                {
                    UIController.Instance.ShowGameMsg("Can't lock enermy");
                    return;
                }
                
                _enemies.ForEach(item => item.Update());
                _enemies.Sort(); // Sort by distance.

                float currentDistance = _lockedEnemy?.Distance ?? 0.0f;
                
                _lockedEnemy = null;
                foreach (var candidateEnemy in _enemies)
                {
                    if (candidateEnemy.IsLockable)
                    {
                        if (_lockedEnemy == null && candidateEnemy.Distance > currentDistance)
                        {
                            _audioSource.clip = lockAC;
                            _audioSource.Play();
                            
                            candidateEnemy.Enemy.Lock(); // Invoke it's lock method to lock it!
                            _lockedEnemy = candidateEnemy; // Lock the nearest enermy.
                            break;
                        }
                    }
                }

                if (_lockedEnemy == null)
                {
                    foreach (var candidateEnemy in _enemies)
                    {
                        if (candidateEnemy.IsLockable)
                        {
                            candidateEnemy.Enemy.Lock(); // Invoke it's lock method to lock it!
                            _lockedEnemy = candidateEnemy; // Lock the nearest enermy.
                            break;
                        }
                    }
                }
                
                if (_lockedEnemy == null)
                {
                    UIController.Instance.ShowGameMsg("Can't lock enermy");
                }
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = false;
        }
    }
}