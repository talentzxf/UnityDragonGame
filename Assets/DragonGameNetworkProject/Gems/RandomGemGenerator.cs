using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomGemGenerator : NetworkBehaviour
{
    [SerializeField] private float maxRange = 5.0f;
    [SerializeField] private List<GameObject> gems;

    [SerializeField] private int totalCount = 5;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }

    public override void Spawned()
    {
        base.Spawned();

        if (Runner.IsSharedModeMasterClient)
        {
            for (int i = 0; i < totalCount; i++)
            {
                int randomIdx = Random.Range(0, gems.Count);
                GameObject gemPrefab = gems[randomIdx];
                Vector3 randomPosition = Random.insideUnitSphere * maxRange;
                Vector3 position = transform.position + randomPosition;
                NetworkObject gem = null;
                try
                {
                   gem = Runner.Spawn(gemPrefab, position);
                }
                catch (Exception)
                {
                    Debug.LogError("Can't spawn the Gem!");
                }
                
                if(gem != null)
                    gem.transform.parent = transform;
            }
        }
    }
}
