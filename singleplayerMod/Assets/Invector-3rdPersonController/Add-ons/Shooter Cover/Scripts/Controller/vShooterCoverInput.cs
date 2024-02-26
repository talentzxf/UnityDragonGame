using Invector.vCharacterController;
namespace Invector.vShooter
{
    public class vShooterCoverInput : vShooterMeleeInput
    {
        //vGetCover cover;
        //protected override void Start()
        //{
        //    base.Start();
        //    cover = GetComponent<vGetCover>();
        //}
        ////protected override void ApplyOffsetToTargetBone(IKOffsetTransform iKOffset, Transform target, bool isValid)
        ////{
        ////    var local = OverBarrierCondition && target.gameObject.name.ToLower().Contains("right") ? target.InverseTransformDirection((Vector3.up+transform.right) * .25f) : Vector3.zero;
        ////    target.localPosition = Vector3.Lerp(target.localPosition, isValid ? iKOffset.position + local : Vector3.zero, 10f * vTime.deltaTime);
        ////    target.localRotation = Quaternion.Lerp(target.localRotation, isValid ? Quaternion.Euler(iKOffset.eulerAngles) : Quaternion.Euler(Vector3.zero), 10f * vTime.deltaTime);
        ////}

        //private bool OverBarrierCondition
        //{
        //    get => IsCrouching && cover.inCover && _aimTimming > 0 && !isAimingByInput &&  cover.currentCornerWeight < .1f;
        //}
    }
}
