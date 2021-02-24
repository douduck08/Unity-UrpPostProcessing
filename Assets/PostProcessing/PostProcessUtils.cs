using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class ShaderIDs {
    public static readonly int MainTex = Shader.PropertyToID ("_MainTex");
}

public static class PostProcessUtils {

    static Mesh s_FullscreenTriangle;

    public static Mesh fullscreenTriangle {
        get {
            if (s_FullscreenTriangle != null)
                return s_FullscreenTriangle;

            s_FullscreenTriangle = new Mesh { name = "Fullscreen Triangle" };

            // Because we have to support older platforms (GLES2/3, DX9 etc) we can't do all of
            // this directly in the vertex shader using vertex ids :(
            s_FullscreenTriangle.SetVertices (new List<Vector3>
            {
                    new Vector3(-1f, -1f, 0f),
                    new Vector3(-1f,  3f, 0f),
                    new Vector3( 3f, -1f, 0f)
                });
            s_FullscreenTriangle.SetIndices (new[] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);
            s_FullscreenTriangle.UploadMeshData (false);

            return s_FullscreenTriangle;
        }
    }

    public static void BlitFullscreenTriangle (this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int pass = 0, bool clear = false) {
        cmd.SetGlobalTexture (ShaderIDs.MainTex, source);
        cmd.SetRenderTarget (new RenderTargetIdentifier (destination, 0, CubemapFace.Unknown, -1),
                    RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                    RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);

        if (clear) {
            cmd.ClearRenderTarget (true, true, Color.clear);
        }

        cmd.DrawMesh (fullscreenTriangle, Matrix4x4.identity, material, 0, pass);
    }
}
