using UnityEngine;
namespace Invector
{
    using vCharacterController;

    public class vCheckOnEnterCoverSide : MonoBehaviour
    {
        public float angleMargin = 15;
        public UnityEngine.Events.UnityEvent onEnter;
        [vSeparator("Events by Transform side")]
        public UnityEngine.Events.UnityEvent onEnterForward;
        public UnityEngine.Events.UnityEvent onEnterBackward;
        public UnityEngine.Events.UnityEvent onEnterRight;
        public UnityEngine.Events.UnityEvent onEnterLeft;

        private void Start()
        {
            vCoverPoint[] cps = transform.parent.GetComponentsInChildren<vCoverPoint>(true);

            for (int i = 0; i < cps.Length; i++)
            {
                var cp = cps[i];
                cp.onEnterCover.AddListener(OnEnterCover);
            }
        }

        private void OnEnterCover(vCoverPoint coverPoint, GameObject visitor)
        {
            onEnter.Invoke();

            if (Quaternion.LookRotation(transform.forward).eulerAngles.AngleFormOtherEuler(coverPoint.transform.eulerAngles).y.IsInSideRange(-angleMargin, angleMargin))
            {
                onEnterForward.Invoke();
                return;
            }
            if (Quaternion.LookRotation(-transform.forward).eulerAngles.AngleFormOtherEuler(coverPoint.transform.eulerAngles).y.IsInSideRange(-angleMargin, angleMargin))
            {
                onEnterBackward.Invoke();
                return;
            }
            if (Quaternion.LookRotation(transform.right).eulerAngles.AngleFormOtherEuler(coverPoint.transform.eulerAngles).y.IsInSideRange(-angleMargin, angleMargin))
            {
                onEnterRight.Invoke();
                return;
            }
            if (Quaternion.LookRotation(-transform.right).eulerAngles.AngleFormOtherEuler(coverPoint.transform.eulerAngles).y.IsInSideRange(-angleMargin, angleMargin))
            {
                onEnterLeft.Invoke();
                return;
            }

        }

    }
}
