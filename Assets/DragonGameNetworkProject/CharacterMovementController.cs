using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class CharacterMovementController : MonoBehaviour
    {
        private AbstractCharacterMovement[] movments;

        private void Awake()
        {
            movments = GetComponents<AbstractCharacterMovement>();
        }

        public T GetMovement<T>() where T:AbstractCharacterMovement
        {
            foreach (var movement in movments)
            {
                if (movement.IsType<T>())
                {
                    return movement as T;
                }                
            }

            return default;
        }
    }
}