using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{

    
    public class DragonAvatarController: CharacterMovementController
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(DragonAvatarController))]
        public class DragonAvatarControllerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                DragonAvatarController dragonAvatarController = target as DragonAvatarController;
                if (GUILayout.Button("Setup All Colliders"))
                {
                    Collider[] colliders = dragonAvatarController.GetComponentsInChildren<Collider>(true);
                    foreach (var collider in colliders)
                    {
                        collider.gameObject.tag = "Player";
                    }
                }
            }
        }
#endif
    }
}