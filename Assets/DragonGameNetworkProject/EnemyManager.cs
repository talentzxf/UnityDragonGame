using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
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
                    _playerTransform = EnemyManager.Instance.LocalPlayer.avatarGO.transform;
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

        public static EnemyManager Instance => _instance;

        private CharacterMovementController _localPlayer;

        public CharacterMovementController LocalPlayer => _localPlayer;

        public void SetLocalPlayer(CharacterMovementController localPlayer)
        {
            _localPlayer = localPlayer;
        }

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
            
            if (Input.GetKeyDown(KeyCode.Tab)) // Every time press tab to lock the nearest visible enermy.
            {
                _enemies.RemoveAll(item => item == null || item.Enemy == null);
                if (_enemies.Count == 0)
                {
                    UIController.Instance.ShowGameMsg("Can't lock enermy");
                    return;
                }
                
                _enemies.ForEach(item => item.Update());
                _enemies.Sort(); // Sort by distance.
                
                _lockedEnemy = null;
                foreach (var candidateEnemy in _enemies)
                {
                    if (candidateEnemy.IsLockable)
                    {
                        candidateEnemy.Enemy.Lock(); // Invoke it's lock method to lock it!
                        if (_lockedEnemy == null)
                        {
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
        }
    }
}