using UnityEngine;
using UnityEngine.Rendering;

public static class RendererExtensions
{
	public static void TurnOffUnityFeatures(this Renderer renderer)
	{
		renderer.receiveShadows = false;
		renderer.shadowCastingMode = ShadowCastingMode.Off;
		renderer.lightProbeUsage = LightProbeUsage.Off;
		renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		renderer.allowOcclusionWhenDynamic = false;
		renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

		var skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
		if (skinnedMeshRenderer != null)
		{
			skinnedMeshRenderer.skinnedMotionVectors = false;
			skinnedMeshRenderer.updateWhenOffscreen = true;
		}
	}
}