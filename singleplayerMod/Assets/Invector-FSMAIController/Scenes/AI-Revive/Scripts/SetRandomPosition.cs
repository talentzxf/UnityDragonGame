using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomPosition : MonoBehaviour
{
    public Transform[] possiblePoints;

    System.Random random;
    // Start is called before the first frame update
    void Awake()
    {
        random = new System.Random(Random.Range(0, GetInstanceID()));      
    }

    // Update is called once per frame
   public void SetRandomPositionToTransform(Transform target)
    {
        Transform newTarget = possiblePoints[random.Next(possiblePoints.Length)];
        if(newTarget)
        {
            target.SetPositionAndRotation(newTarget.position, newTarget.rotation);
        }
    }
}
