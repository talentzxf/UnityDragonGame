using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI
{
    [System.Serializable]
    public partial class vAIReceivedDamegeInfo
    {
        public vAIReceivedDamegeInfo()
        {
            lasType = "unnamed";
        }
        [vReadOnly(false)] public bool isValid;
        [vReadOnly(false)] public float lastValue;
        [vReadOnly(false)] public string lasType = "unnamed";
        [vReadOnly(false)] public Transform lastSender;
        [vReadOnly(false)] public int massiveCount;
        [vReadOnly(false)] public float massiveValue;

        protected float lastValidDamage;
        float _massiveTime;
        public void Update()
        {
            _massiveTime -= Time.deltaTime;
            if (_massiveTime <= 0)
            {
                _massiveTime = 0;
                if (massiveValue > 0) massiveValue -= 1;
                if (massiveCount > 0) massiveCount -= 1;
            }
            isValid = lastValidDamage > Time.time;
        }

        public void UpdateDamage(vDamage damage, float validDamageTime = 2f)
        {
            if (damage == null) return;
            lastValidDamage = Time.time + validDamageTime;
            _massiveTime += Time.deltaTime;
            massiveCount++;
            lastValue = damage.damageValue;
            massiveValue += lastValue;
            lastSender = damage.sender;
            lasType = string.IsNullOrEmpty(damage.damageType) ? "unnamed" : damage.damageType;
        }
    }
}