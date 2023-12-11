using UnityEngine;

public class ClimbDragon : MonoBehaviour
{
    private Transform dragonTransform;
    private void Start()
    {
        dragonTransform = transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterLocomotion characterLocomotion = other.GetComponent<CharacterLocomotion>();
            
            characterLocomotion.Climb(transform.position, dragonTransform.gameObject); }
    }
}
