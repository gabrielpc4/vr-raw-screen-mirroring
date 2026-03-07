using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

[RequireComponent(typeof(Camera))]
public class BlindMonitor : MonoBehaviour
{
    public Texture2D sourceTexture;
    public Shader blitShader;
    private Material blitMaterial;
    private GameObject canvasGO;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        
        // Ensure camera is fixed at origin
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        // Setup Shader and Material
        if (blitShader == null)
        {
            blitShader = Shader.Find("Hidden/BlindMonitorUI");
        }
        
        if (blitShader != null)
        {
            blitMaterial = new Material(blitShader);
        }

        // Create a Canvas for UI-based rendering (reliable for VR both eyes)
        canvasGO = new GameObject("BlindMonitorCanvas");
        canvasGO.transform.SetParent(this.transform, false);
        
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 1.0f; // Near enough to fill the view

        // Use a RawImage to display the texture
        GameObject rawImageGO = new GameObject("MonitorImage");
        rawImageGO.transform.SetParent(canvasGO.transform, false);
        
        RawImage rawImage = rawImageGO.AddComponent<RawImage>();
        rawImage.texture = sourceTexture;
        rawImage.material = blitMaterial;
        
        // Make RawImage fill the whole screen
        RectTransform rt = rawImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    void LateUpdate()
    {
        // Force the camera to stay at origin (neutralize tracking)
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    void OnDestroy()
    {
        if (blitMaterial != null)
        {
            Destroy(blitMaterial);
        }
    }
}
