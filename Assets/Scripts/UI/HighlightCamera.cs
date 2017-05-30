using UnityEngine;
using System.Collections;

public class HighlightCamera : MonoBehaviour
{
    public Material outlineMaterial;
    public Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, outlineMaterial);
    }
}
