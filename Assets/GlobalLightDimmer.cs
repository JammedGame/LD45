using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlobalLightDimmer : MonoBehaviour
{
    public Light2D Light;

    void Update()
    {
        if (Game.Player == null) return;
        Light.intensity = Game.Player.HasEyes ? 0.05f : 0f;
    }
}
