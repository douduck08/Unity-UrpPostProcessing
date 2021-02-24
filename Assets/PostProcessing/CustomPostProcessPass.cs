using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

public class CustomPostProcessPass : ScriptableRenderPass {

    static readonly int finalTarget = Shader.PropertyToID ("_AfterPostProcessTexture");

    enum PostPorcess {
        HueShift,
    }

    HueShift hueShift;

    Material hueShiftMaterial;

    public CustomPostProcessPass (CustomPostProcess.Shaders shaders) {
        this.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        hueShiftMaterial = CoreUtils.CreateEngineMaterial (shaders.hueShiftShader);
    }

    public override void Execute (ScriptableRenderContext context, ref RenderingData renderingData) {
        var stack = VolumeManager.instance.stack;
        hueShift = stack.GetComponent<HueShift> ();

        var cmd = CommandBufferPool.Get ("Custom PostProcessing");
        Render (cmd, ref renderingData);
        context.ExecuteCommandBuffer (cmd);
        cmd.Clear ();
        CommandBufferPool.Release (cmd);
    }

    void Render (CommandBuffer cmd, ref RenderingData renderingData) {
        ref var cameraData = ref renderingData.cameraData;
        var isSceneViewCamera = cameraData.isSceneViewCamera;
        var pixelRect = cameraData.camera.pixelRect;
        var width = (int)pixelRect.width;
        var height = (int)pixelRect.height;

        var source = Shader.PropertyToID ("cycleRT1");
        var destination = Shader.PropertyToID ("cycleRT2");

        int GetSource () => source;
        int GetDestination () => destination;
        void Swap () => CoreUtils.Swap (ref source, ref destination);

        cmd.GetTemporaryRT (source, width, height, 0, FilterMode.Bilinear, GraphicsFormat.R8G8B8A8_UNorm);
        cmd.GetTemporaryRT (destination, width, height, 0, FilterMode.Bilinear, GraphicsFormat.R8G8B8A8_UNorm);
        cmd.Blit (BuiltinRenderTextureType.CurrentActive, source);

        if (hueShift.IsActive () && !isSceneViewCamera) {
            using (new ProfilingScope (cmd, ProfilingSampler.Get (PostPorcess.HueShift))) {
                DoHueShift (cmd, GetSource (), GetDestination ());
                Swap ();
            }
        }

        cmd.Blit (GetSource (), finalTarget);
        cmd.ReleaseTemporaryRT (source);
        cmd.ReleaseTemporaryRT (destination);
    }

    void DoHueShift (CommandBuffer cmd, int source, int destination) {
        hueShiftMaterial.SetFloat ("_Scalar", hueShift.percentage.value);
        cmd.BlitFullscreenTriangle (source, destination, hueShiftMaterial);
    }
}
