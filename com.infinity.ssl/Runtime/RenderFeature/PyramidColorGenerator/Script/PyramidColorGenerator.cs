using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace InfinityExtend.Rendering.Runtime
{
    public static class PyramidColorUniform
    {
        public static int PrevLevelColor = Shader.PropertyToID("_Source");
        public static int CurrLevelColor = Shader.PropertyToID("_Result");
        public static int PrevCurr_Size = Shader.PropertyToID("_Size");
        public static int ColorPyramidNumLOD = Shader.PropertyToID("ColorPyramidNumLOD");
    }

    public static class PyramidColorGenerator
    {
        private static ComputeShader PyramidColorShader {
            get {
                return Resources.Load<ComputeShader>("Shaders/GaussianDownsampleShader");
            }
        }

        public static void ColorPyramidInit(ref int[] ColorPyramidMipIDs)
        {
            if (ColorPyramidMipIDs == null || ColorPyramidMipIDs.Length == 0) {
                ColorPyramidMipIDs = new int[12];

                for (int i = 0; i < 12; i++) {
                    ColorPyramidMipIDs[i] = Shader.PropertyToID("_SSSRGaussianMip" + i);
                }
            }
        }

        public static void ColorPyramidUpdate(ref int[] ColorPyramidMipIDs, ref int2 ScreenSize, RenderTargetIdentifier DstRT , CommandBuffer CommandList)
        {
            int ColorPyramidCount = Mathf.FloorToInt(Mathf.Log(ScreenSize.x, 2) - 3);
            ColorPyramidCount = Mathf.Min(ColorPyramidCount, 12);
            CommandList.SetGlobalFloat(PyramidColorUniform.ColorPyramidNumLOD, (float)ColorPyramidCount);
            RenderTargetIdentifier PrevColorPyramid = DstRT;
            int2 ColorPyramidSize = ScreenSize;
            for (int i = 0; i < ColorPyramidCount; i++) {
                ColorPyramidSize.x >>= 1;
                ColorPyramidSize.y >>= 1;

                CommandList.GetTemporaryRT(ColorPyramidMipIDs[i], ColorPyramidSize.x, ColorPyramidSize.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default, 1, true);
                CommandList.SetComputeTextureParam(PyramidColorShader, 0, PyramidColorUniform.PrevLevelColor, PrevColorPyramid);
                CommandList.SetComputeTextureParam(PyramidColorShader, 0, PyramidColorUniform.CurrLevelColor, ColorPyramidMipIDs[i]);
                CommandList.SetComputeVectorParam(PyramidColorShader, PyramidColorUniform.PrevCurr_Size, new float4(ColorPyramidSize.x, ColorPyramidSize.y, 1f / ColorPyramidSize.x, 1f / ColorPyramidSize.y));
                CommandList.DispatchCompute(PyramidColorShader, 0, Mathf.CeilToInt(ColorPyramidSize.x / 8f), Mathf.CeilToInt(ColorPyramidSize.y / 8f), 1);
                CommandList.CopyTexture(ColorPyramidMipIDs[i], 0, 0, DstRT, 0, i + 1);

                PrevColorPyramid = ColorPyramidMipIDs[i];
            } for (int i = 0; i < ColorPyramidCount; i++) {
                CommandList.ReleaseTemporaryRT(ColorPyramidMipIDs[i]);
            }
        }
    }
}
