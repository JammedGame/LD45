using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Animator))]
public class PlayerVisual : BattleObject3D
{
    public Light2D Light2D;
    public float NoEyesLightScale = 0.1f;
    public float NoEyesLightIntensity = 0.33f;

    public override void Sync(float dT)
    {
        base.Sync(dT);

        var player = (Player)Data;
        Animator.SetInteger("ActionType", (int)player.CurrentAnimation);

        var targetLightScale = player.HasEyes ? Vector3.one : NoEyesLightScale * Vector3.one;
        var targetLightIntensity = player.HasEyes ? 1 : NoEyesLightIntensity;

        Light2D.transform.localScale = Vector3.Lerp(Light2D.transform.localScale, targetLightScale, dT * 3);
        Light2D.intensity = Mathf.MoveTowards(Light2D.intensity, targetLightIntensity, dT * 3);
    }

    public override void Init(BattleObject data)
    {
        var player = (Player)Data;
        Light2D.transform.localScale = player.HasEyes ? Vector3.one : NoEyesLightScale * Vector3.one;
        Light2D.intensity = player.HasEyes ? 1 : NoEyesLightIntensity;
    }
}