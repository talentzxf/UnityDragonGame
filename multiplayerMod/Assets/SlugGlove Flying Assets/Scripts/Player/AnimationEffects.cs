using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEffects : MonoBehaviour
{
    private PlayerVisuals Vis;

    private void Start()
    {
        Vis = GetComponentInParent<PlayerVisuals>();
    }

    void Step()
    {
        Vis.Step();
    }

    void Flap()
    {

    }
}
