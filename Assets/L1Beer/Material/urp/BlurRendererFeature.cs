using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class BlurSettings
    {
        public int iterations = 10;  // ģ����������
        public float blurSpread = 0.6f;  // ÿ�ε�������ɢ��
        public Shader blurShader;  // ģ��Shader
    }

    public BlurSettings settings = new BlurSettings();

    class BlurRenderPass : ScriptableRenderPass
    {
        ProfilingSampler m_ProfilingSampler = new("BlurRenderPass");


        private Material blurMaterial;
        private RTHandle source;
        private RTHandle temporaryTexture1, temporaryTexture2;
        private int width, height;
        private BlurSettings settings;

        public BlurRenderPass(BlurSettings blurSettings)
        {
            settings = blurSettings;
            blurMaterial = new Material(settings.blurShader) { hideFlags = HideFlags.DontSave };
        }

        // ִ�и�˹ģ������
        public void Setup(RTHandle source)
        {
            this.source = source;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // ��������Ŀ�Ⱥ͸߶�
            width = renderingData.cameraData.cameraTargetDescriptor.width / 4;  // �²���
            height = renderingData.cameraData.cameraTargetDescriptor.height / 4;

            //��ʼ����������ͼ
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.width = width;
            desc.height = height;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref temporaryTexture1, desc , FilterMode.Bilinear);//����
            RenderingUtils.ReAllocateIfNeeded(ref temporaryTexture2, desc , FilterMode.Bilinear);
            cmd.SetGlobalTexture("_TemporaryTexture1", temporaryTexture1.nameID);//����ͨ������_MyColorTex������shader���ҵ�_GrabTex
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("BlurRenderPass");

            using (new UnityEngine.Rendering.ProfilingScope(cmd, m_ProfilingSampler))
            {
                //kawase blur

                // �²�������ʱ����
                blurMaterial.SetFloat("_BlurRange", 0.0f);
                cmd.Blit(source, temporaryTexture1, blurMaterial , 0);
                //Blitter.BlitCameraTexture(cmd, source, temporaryTexture1,blurMaterial,0);

                // ִ�ж��ģ������
                for (int i = 0; i < settings.iterations; i++)
                {
                    //float offset = 0.5f + i * settings.blurSpread;
                    float blurRange = (i + 1) * settings.blurSpread;

                    // ����ģ��ƫ����
                    //blurMaterial.SetFloat("_BlurOffsets", offset);
                    blurMaterial.SetFloat("_BlurRange", blurRange);

                    // ʹ�� Graphics.Blit ִ��ģ������
                    cmd.Blit(temporaryTexture1, source, blurMaterial,0);
                    //Blitter.BlitCameraTexture(cmd, temporaryTexture1, temporaryTexture2, blurMaterial, 0);                   
                    //Blitter.BlitCameraTexture(cmd, temporaryTexture2, temporaryTexture1);

                    //var temRT = temporaryTexture1;
                    //temporaryTexture1 = temporaryTexture2;
                    //temporaryTexture2 = temRT;

                    // ����ʱ������Ϊ���������һ�ε���
                    cmd.Blit(source, temporaryTexture1);
                    //Blitter.BlitCameraTexture(cmd, source, temporaryTexture);

                }
                //CoreUtils.SetRenderTarget(cmd, renderingData.cameraData.targetTexture);
                
                //Blitter.BlitCameraTexture(cmd, temporaryTexture1, source, blurMaterial, 0);

            }
            // ִ������
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            cmd.Dispose();
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public void OnDispose()
        {
            temporaryTexture1?.Release();
            temporaryTexture2?.Release();
        }
    }

    private BlurRenderPass blurRenderPass;

    public override void Create()
    {
        blurRenderPass = new BlurRenderPass(settings);
        blurRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;  // ����Ⱦ͸������֮��ִ��
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //if
        Camera camera = renderingData.cameraData.camera;
        if (!camera.CompareTag("BlurCamera")) return;

        // �� `ScriptableRenderPass` ��ӵ���Ⱦ������
        renderer.EnqueuePass(blurRenderPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        //if
        Camera camera = renderingData.cameraData.camera;
        if (!camera.CompareTag("BlurCamera")) return;

        // ʹ�� RTHandle ��ȡ��ǰ��ȾĿ��
        RTHandle cameraColorHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
        blurRenderPass.Setup(cameraColorHandle);
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        blurRenderPass.OnDispose();
    }
}
