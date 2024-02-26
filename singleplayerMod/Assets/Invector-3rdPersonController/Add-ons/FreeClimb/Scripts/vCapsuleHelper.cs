using UnityEngine;
namespace Invector
{
    public static class vCapsuleHelper
    {
        public static bool CheckCapsule(this CapsuleCollider capsule, Vector3 dir, out RaycastHit hit, LayerMask mask, bool drawGizmos = false)
        {
            return capsule.CheckCapsule(dir, out hit, capsule.radius * 0.5f, mask, drawGizmos);
        }
        public static bool CheckCapsule(this CapsuleCollider capsule, Vector3 dir, LayerMask mask, bool drawGizmos = false)
        {
            return capsule.CheckCapsule(dir, capsule.radius * 0.5f, mask, drawGizmos);
        }

        public static bool CheckCapsule(this CapsuleCollider capsule, Vector3 dir, out RaycastHit hit, float distance, LayerMask mask, bool drawGizmos = false)
        {
            return capsule.CheckCapsule(dir, out hit, capsule.radius, distance, mask, drawGizmos);
        }
        public static bool CheckCapsule(this CapsuleCollider capsule, Vector3 dir, float distance, LayerMask mask, bool drawGizmos = false)
        {

            return capsule.CheckCapsule(dir, capsule.radius, distance, mask, drawGizmos);
        }

        public static bool CheckCapsule(this CapsuleCollider capsule, Vector3 dir, out RaycastHit hit, float radius, float distance, LayerMask mask, bool drawGizmos = false)
        {
            var pCenter = capsule.transform.TransformPoint(capsule.center);
            var p1 = pCenter + capsule.transform.up * ((capsule.height * 0.5f) - capsule.radius);
            var p2 = pCenter - capsule.transform.up * ((capsule.height * 0.5f) - capsule.radius);


            var check = false;
            if (Physics.CapsuleCast(p1, p2, radius, dir, out hit, distance, mask))
            {
                check = true;
            }
            if (drawGizmos)
            {
                DraCapsule(pCenter, capsule.transform.forward, capsule.transform.up, capsule.transform.right, capsule.height, radius, check ? Color.red : Color.green);
            }

            return check;
        }
        public static bool CheckCapsule(this CapsuleCollider capsule, Vector3 dir, float radius, float distance, LayerMask mask, bool drawGizmos = false)
        {

            var pCenter = capsule.transform.TransformPoint(capsule.center);
            var p1 = pCenter + capsule.transform.up * ((capsule.height * 0.5f) - capsule.radius);// + capsule.transform.forward * 0.3f;
            var p2 = pCenter - capsule.transform.up * ((capsule.height * 0.5f) - capsule.radius);


            var check = false;

            if (Physics.CapsuleCast(p1, p2, radius, dir, distance, mask, QueryTriggerInteraction.Collide))
            {
                check = true;
            }
            if (drawGizmos)
            {
                DraCapsule(pCenter, capsule.transform.forward, capsule.transform.up, capsule.transform.right, capsule.height, radius, check ? Color.red : Color.green);
            }

            return check;
        }

        public static void DraCapsule(this CapsuleCollider capsule, Color color)
        {

            DraCapsule(capsule.transform.position + capsule.center, capsule.transform.forward, capsule.transform.up, capsule.transform.right, capsule.height, capsule.radius, color);
        }
        public static void DraCapsule(Vector3 center, Vector3 forward, Vector3 up, Vector3 right, float height, float radius, Color color)
        {

            vDebug.DrawSphere(center + up * (height * 0.5f - radius), radius, color);
            vDebug.DrawSphere(center - up * (height * 0.5f - radius), radius, color);
            var p1 = center + forward * radius;
            var p2 = center - forward * radius;
            var p3 = center + right * radius;
            var p4 = center - right * radius;
            Debug.DrawLine(p1 + up * (height * 0.5f - radius), p1 - up * (height * 0.5f - radius), color);
            Debug.DrawLine(p2 + up * (height * 0.5f - radius), p2 - up * (height * 0.5f - radius), color);
            Debug.DrawLine(p3 + up * (height * 0.5f - radius), p3 - up * (height * 0.5f - radius), color);
            Debug.DrawLine(p4 + up * (height * 0.5f - radius), p4 - up * (height * 0.5f - radius), color);
        }

    }
}