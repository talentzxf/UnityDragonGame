using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;

public abstract class AbstractGemGenerator : MonoBehaviour
{
    [SerializeField] private float activateAfterSeconds = -1.0f;
    

#if UNITY_EDITOR
    [SerializeField] protected List<GameObject> gems;
    public abstract void GenerateGems();

    public void DeleteAllGems()
    {
        int remainChild = 0;
        
        for (int childIdx = 0; childIdx < transform.childCount; childIdx++)
        {
            GameObject go = transform.GetChild(childIdx).gameObject;
            if (!CanDelete(go))
            {
                remainChild++;
            }
        }
        
        while (transform.childCount > remainChild)
        {
            for (int childIdx = 0; childIdx < transform.childCount; childIdx++)
            {
                GameObject go = transform.GetChild(childIdx).gameObject;
                if (CanDelete(go))
                {
                    Undo.DestroyObjectImmediate(go);                    
                }
            }
        }
    }

    protected virtual bool CanDelete(GameObject go)
    {
        return true;
    }

    protected void GenerateGemAtPosition(Vector3 position)
    {
        int randomIdx = Random.Range(0, gems.Count);
        GameObject gemPrefab = gems[randomIdx];
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
#endif
}

#if UNITY_EDITOR
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
#endif

public class RandomGemGenerator : AbstractGemGenerator
{
#if UNITY_EDITOR
    [SerializeField] private float radius = 5.0f;

    [SerializeField] private int totalCount = 5;

    [CustomEditor(typeof(RandomGemGenerator))]
    class RandomGemGeneratorEditor : GemGeneratorEditor
    {
    }

    public override void GenerateGems()
    {
        for (int i = 0; i < totalCount; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * radius;
            Vector3 position = transform.position + randomPosition;
            
            GenerateGemAtPosition(position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}