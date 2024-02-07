using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [Header("Visual")]
    public Transform MovementMesh; //what we are checking the ground based on
    public Transform LeftWing;
    public Transform RightWing;
    public Transform HipsPos;
    private float TimeBtwFallingEffects;

    [Header("VisualFx")]
    public GameObject WalkFx;
    public GameObject JumpFx;
    public GameObject LandingFx;

    [Header("Flying Fx")]
    public GameObject WingTrail;
    private float TimeBtwFlyFx;

    [Header("Audio")]
    public GameObject JumpAudio;

    [Header("WindFx")]
    public GameObject WindFx;
    public AudioSource WindAudio;
    public float WindAudioMax;
    [HideInInspector]
    public float WindLerpAmt;
    public float WindLerpSpeed;

    void Start()
    {
        TimeBtwFallingEffects = 1.8f;
    }

    public void Step()
    {
        if (WalkFx)
            return;

        Vector3 pos = transform.position;
        GameObject obj = Instantiate(WalkFx, pos, Quaternion.identity);
    }

    public void Jump()
    {
        if (JumpFx)
            Instantiate(JumpFx, transform.position, MovementMesh.transform.rotation);

        if (JumpAudio)
            Instantiate(JumpAudio, transform.position, Quaternion.identity).GetComponent<PlayAudio>();
    }
    public void Landing()
    {
        if (LandingFx)
        {
            Instantiate(LandingFx, transform.position, MovementMesh.transform.rotation);

        }
    }

    //effect checks
    public void SetFallingEffects(float Amt)
    {
        TimeBtwFallingEffects = Amt;
    }

    public void FallEffectCheck(float D)
    {
        if (!WindFx)
            return;

        if (TimeBtwFallingEffects > 0)
        {
            TimeBtwFallingEffects -= D;
            return;
        }

        GameObject Wind = Instantiate(WindFx, transform.position, transform.rotation);
        Wind.transform.parent = this.transform;

        TimeBtwFallingEffects = Random.Range(0.6f, 1.2f);

    }

    public void FlyingFxTimer(float D)
    {
        if (TimeBtwFlyFx > 0)
            TimeBtwFlyFx -= D;
        else
        {
            TimeBtwFlyFx = 0.5f;

            //wing trails
            if(WingTrail)
            {
                if(LeftWing)
                {
                    GameObject LeftWingFX = Instantiate(WingTrail, LeftWing.transform.position, Quaternion.identity);
                    LeftWingFX.transform.parent = LeftWing;
                }
                if(RightWing)
                {
                    GameObject RightWingFx = Instantiate(WingTrail, RightWing.transform.position, Quaternion.identity);
                    RightWingFx.transform.parent = RightWing;
                }
            }
        }
    }

    public void WindAudioSetting(float D, float VelocityMagnitude)
    {
        if (!WindAudio)
            return;

        float LerpAmt = VelocityMagnitude / 40;

        WindLerpAmt = Mathf.Lerp(WindLerpAmt, LerpAmt, D * WindLerpSpeed);

        WindAudio.volume = Mathf.Lerp(0, WindAudioMax, WindLerpAmt);
    }


}
