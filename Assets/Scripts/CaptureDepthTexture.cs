using System;
using Unity.VisualScripting;
using UnityEngine;

public class CaptureDepthTexture : MonoBehaviour
{
    private static CaptureDepthTexture instance;

    public static CaptureDepthTexture Instance => instance ??= FindObjectOfType<CaptureDepthTexture>();
    private Camera cam;
    private Camera Cam => cam ??= GetComponent<Camera>();

    private int requestedWidth;
    private int requestedHeight;

    private Shader _shader;

    private Shader shader
    {
        get { return _shader != null ? _shader : (_shader = Shader.Find("Custom/RenderDepth")); }
    }

    private Material _material;

    private Material material
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }

            return _material;
        }
    }

    private void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            print("System doesn't support image effects");
            enabled = false;
            return;
        }

        if (shader == null || !shader.isSupported)
        {
            enabled = false;
            print("Shader " + shader.name + " is not supported");
            return;
        }

        // turn on depth rendering for the camera so that the shader can access it via _CameraDepthTexture
        Cam.depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnPostRender()
    {
        if (OnDepthTextureGenerated != null)
        {
            var rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

            var renderCamera = new GameObject().AddComponent<Camera>();
            renderCamera.CopyFrom(Cam);
            renderCamera.transform.position = Cam.transform.position;
            renderCamera.transform.rotation = Cam.transform.rotation;

            renderCamera.targetTexture = rt;
            renderCamera.Render();

            Texture2D virtualPhoto =
                new Texture2D(requestedWidth, requestedHeight, TextureFormat.RGB24, false);
            virtualPhoto.ReadPixels(new Rect(0, 0, requestedWidth, requestedHeight), 0, 0);
            virtualPhoto.Apply();
            Graphics.Blit(virtualPhoto, rt, material);
            virtualPhoto.ReadPixels(new Rect(0, 0, requestedWidth, requestedHeight), 0, 0);
            virtualPhoto.Apply();
            renderCamera.targetTexture = null;
            Destroy(renderCamera.gameObject);
            OnDepthTextureGenerated?.Invoke(virtualPhoto);
            OnDepthTextureGenerated = null;
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