using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostProcess : ScriptableRendererFeature {

    [System.Serializable]
    public class Shaders {
        public Shader hueShiftShader;
    }

    public Shaders shaders;

    CustomPostProcessPass customPostProcessPass;

    public override void Create () {
        customPostProcessPass = new CustomPostProcessPass (shaders);
    }

    public override void AddRenderPasses (ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass (customPostProcessPass);
    }
}
