using UnityEngine;
using UnityEngine.SceneManagement;

namespace Invector
{
    public class vCoverMiniGameManager : MonoBehaviour
    {
        public bool getChildrenDetectionController;
        public vDetectionController[] securityCameras;
        public UnityEngine.Events.UnityEvent onPlayerIsFound;
        public UnityEngine.Events.UnityEvent onPlayerIsLost;
        void Start()
        {
            if (getChildrenDetectionController)
            {
                securityCameras = GetComponentsInChildren<vDetectionController>();
            }
            for (int i = 0; i < securityCameras.Length; i++)
            {
                var sCamera = securityCameras[i];
                sCamera.onFindTransformTarget.AddListener((Transform t) => { OnPlayerIsFound(); });
                sCamera.onLostTransformTarget.AddListener((Transform t) => { OnPlayerIsLost(); });
            }
        }

        public virtual void OnPlayerIsFound()
        {
            onPlayerIsFound.Invoke();
        }

        public virtual void OnPlayerIsLost()
        {
            onPlayerIsLost.Invoke();
        }

        public virtual void ResetScene()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}