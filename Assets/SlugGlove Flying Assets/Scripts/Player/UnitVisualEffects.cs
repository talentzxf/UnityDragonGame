using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitVisualEffects : ScriptableObject
{
    //used to hold access to all visual effects a unit needs
    [Header("VisualFx")]
    public GameObject DustFx;
    public GameObject JumpFx;
    public GameObject DiveFx;
    public GameObject BackDiveFx;
    public GameObject LandingFx;
    public GameObject DoubleJumpFx;
    public GameObject CrouchJumpFx;

    [Header("Flying Fx")]
    public GameObject WingTrail;
    public GameObject FlyingLegsFx;
    private float TimeBtwFlyFx;

    [Header("Audio")]
    public GameObject JumpAudio;
    public GameObject JumpLargeAudio;

    [Header("Hurt Effect")]
    public GameObject DamagedAudio;
    public GameObject DamagedEffect;

}
