using UnityEngine;
using UnityEngine.UI;
using Klak.Ndi;

[RequireComponent(typeof(Camera))]
public class BlindMonitor : MonoBehaviour
{
    [Header("NDI Configuration")]
    public NdiReceiver ndiReceiver;
    public Shader blitShader;
    
    private Material blitMaterial;
    private GameObject canvasGO;
    private RawImage rawImage;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        
        // 1. Setup Shader and Material for 1:1 Eye Splitting
        if (blitShader == null)
        {
            blitShader = Shader.Find("Hidden/BlindMonitorUI");
        }
        
        if (blitShader != null)
        {
            blitMaterial = new Material(blitShader);
        }
        else
        {
            Debug.LogError("BlindMonitorUI shader not found! Ensure it is in the project.");
        }

        // 2. Create the Canvas (Pinned to Eyes)
        canvasGO = new GameObject("BlindMonitorCanvas");
        canvasGO.transform.SetParent(this.transform, false);
        
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 1.0f; 

        // 3. Setup the RawImage to fill the viewport
        GameObject rawImageGO = new GameObject("MonitorImage");
        rawImageGO.transform.SetParent(canvasGO.transform, false);
        
        rawImage = rawImageGO.AddComponent<RawImage>();
        rawImage.material = blitMaterial;
        
        RectTransform rt = rawImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    void Update()
    {
        // 4. Neutralize Headset Tracking (Force 1:1 mapping)
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // 5. Inject the Live NDI Texture
        if (ndiReceiver != null && rawImage != null)
        {
            rawImage.texture = ndiReceiver.texture;
        }
    }

    void OnDestroy()
    {
        if (blitMaterial != null)
        {
            Destroy(blitMaterial);
        }
    }
}
