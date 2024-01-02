using System;
using System.Collections.Generic;using DragonGameNetworkProject;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class AbstractGemGenerator : MonoBehaviour
{
    [SerializeField] protected List<GameObject> gems;

    public abstract void GenerateGems();
    
    public void DeleteAllGems()
    {
        while (transform.childCount != 0)
        {
            for(int childIdx = 0 ; childIdx < transform.childCount; childIdx++)
            {
                Undo.DestroyObjectImmediate(transform.GetChild(childIdx).gameObject);
            }            
        }
    }
}

public class GemGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AbstractGemGenerator gemGenerator = target as AbstractGemGenerator;
        if (GUILayout.Button("Generate"))
        {
            gemGenerator.GenerateGems();
        }

        if (GUILayout.Button("Delete All"))
        {
            gemGenerator.DeleteAllGems();
        }
    }
}

public class RandomGemGenerator : AbstractGemGenerator
{
#if UNITY_EDITOR
    [SerializeField] private float radius = 5.0f;

    [SerializeField] private int totalCount = 5;

    [CustomEditor(typeof(RandomGemGenerator))]
    class RandomGemGeneratorEditor: GemGeneratorEditor{}

    public override void GenerateGems()
    {
        for (int i = 0; i < totalCount; i++)
        {
            int randomIdx = Random.Range(0, gems.Count);
            GameObject gemPrefab = gems[randomIdx];
            Vector3 randomPosition = Random.insideUnitSphere * radius;
            Vector3 position = transform.position + randomPosition;
            GameObject gem = null;
            try
            {
                gem = Instantiate(gemPrefab, position, gemPrefab.transform.rotation);
            }
            catch (Exception)
            {
                Debug.LogError("Can't spawn the Gem!");
            }
        
            if (gem != null)
            {
                gem.transform.parent = transform;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    
#endif
}