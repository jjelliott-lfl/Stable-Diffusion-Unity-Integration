using System;
using UnityEngine;

public class CaptureDepthTexture : MonoBehaviour
{
    private static CaptureDepthTexture instance;

    public static CaptureDepthTexture Instance => instance ??= FindObjectOfType<CaptureDepthTexture>();
    private Camera cam;
    private Camera Cam => cam ??= GetComponent<Camera>();

    private int requestedWidth;
    private int requestedHeight;

    private void OnPostRender()
    {
        if (OnDepthTextureGenerated != null)
        {
            var rt = RenderTexture.GetTemporary(requestedWidth, requestedHeight, 0);

            Cam.targetTexture = rt;
            Cam.RenderWithShader(Shader.Find("Render Depth"), "");
            Texture2D virtualPhoto =
                new Texture2D(requestedWidth, requestedHeight, TextureFormat.RGB24, false);
            virtualPhoto.ReadPixels(new Rect(0, 0, requestedWidth, requestedHeight), 0, 0);
            virtualPhoto.Apply();
            OnDepthTextureGenerated?.Invoke(virtualPhoto);
            Cam.targetTexture = null;
        }
    }

    private Action<Texture2D> OnDepthTextureGenerated;

    public static void RequestDepthTexture(int width, int height, Action<Texture2D> onGenerated)
    {
        Instance.requestedWidth = width;
        Instance.requestedHeight = height;

        Instance.OnDepthTextureGenerated += onGenerated;
    }
}