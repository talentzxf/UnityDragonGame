using System;
using System.Collections.Generic;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class EnermyManager: MonoBehaviour
    {
        private static EnermyManager _instance;

        private List<Enermy> _enermies = new();

        public static EnermyManager Instance => _instance;

        public void RegisterEnermy(Enermy enermy)
        {
            _enermies.Add(enermy);
        }

        private void Update()
        {
            _enermies.RemoveAll(item => item == null);
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