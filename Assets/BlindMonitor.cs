using UnityEngine;
using UnityEngine.UI;
using Klak.Ndi;
using System.Linq;

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
        
        // Ensure camera is clean (Black background, no skybox)
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        // Setup Shader and Material
        if (blitShader == null)
        {
            blitShader = Shader.Find("Hidden/BlindMonitorUI");
        }
        
        if (blitShader != null)
        {
            blitMaterial = new Material(blitShader);
        }

        // Setup Canvas
        canvasGO = new GameObject("BlindMonitorCanvas");
        canvasGO.transform.SetParent(this.transform, false);
        
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 1.0f; 

        // Setup RawImage
        GameObject rawImageGO = new GameObject("MonitorImage");
        rawImageGO.transform.SetParent(canvasGO.transform, false);
        
        rawImage = rawImageGO.AddComponent<RawImage>();
        rawImage.material = blitMaterial;
        rawImage.color = Color.white; // Ensure it's not transparent
        
        RectTransform rt = rawImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        Debug.Log("BlindMonitor: Initialized and waiting for NDI stream...");
    }

    void Update()
    {
        // Force 1:1 Head-Lock
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (ndiReceiver != null && rawImage != null)
        {
            if (ndiReceiver.texture != null)
            {
                if (rawImage.texture == null) 
                    Debug.Log("BlindMonitor: NDI Texture Received!");
                
                rawImage.texture = ndiReceiver.texture;
            }
            else
            {
                // If we have a receiver but no texture, it might be waiting for the source
                // You can check Unity Console to see if it's connected
            }
        }
    }

    void OnDestroy()
    {
        if (blitMaterial != null) Destroy(blitMaterial);
    }
}
