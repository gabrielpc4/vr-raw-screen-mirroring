using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class BlindMonitor : MonoBehaviour
{
    public Texture2D sourceTexture;
    public Shader blitShader;
    
    [Header("Tracking Settings")]
    public bool bypassTracking = false; 
    
    [Header("Virtual Window Settings")]
    public float planeDistance = 1.5f; 
    public float planeWidth = 5.0f;    
    
    private Material blitMaterial;
    private GameObject canvasGO;
    private List<InputDevice> inputDevices = new List<InputDevice>();

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        
        // Setup Shader and Material
        if (blitShader == null)
        {
            blitShader = Shader.Find("Hidden/BlindMonitorUI");
        }
        
        if (blitShader != null)
        {
            blitMaterial = new Material(blitShader);
        }

        // Create the Canvas
        canvasGO = new GameObject("BlindMonitorCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        
        if (bypassTracking)
        {
            // MODE: GLUED TO EYES
            canvasGO.transform.SetParent(this.transform, false);
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 1.0f;
        }
        else
        {
            // MODE: VIRTUAL WINDOW (STAY IN ROOM)
            canvas.renderMode = RenderMode.WorldSpace;
            
            // ENSURE IT HAS NO PARENT
            canvasGO.transform.SetParent(null); 
            
            // Position it at a fixed world coordinate (1.5m ahead of start)
            canvasGO.transform.position = new Vector3(0, 0, planeDistance);
            canvasGO.transform.rotation = Quaternion.identity;

            float scale = planeWidth / 1920f;
            canvasGO.transform.localScale = new Vector3(scale, scale, scale);
        }

        // Setup the RawImage
        GameObject rawImageGO = new GameObject("MonitorImage");
        rawImageGO.transform.SetParent(canvasGO.transform, false);
        
        RawImage rawImage = rawImageGO.AddComponent<RawImage>();
        rawImage.texture = sourceTexture;
        rawImage.material = blitMaterial;
        
        RectTransform rt = rawImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = new Vector2(1920, 1080);
        rt.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (bypassTracking)
        {
            // Lock camera to origin
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            // MANUAL TRACKING: 
            // We manually sync the Unity Camera Transform to the Headset's Pose.
            // This allows the World Space Canvas to stay fixed while we move.
            InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
            
            if (device.isValid)
            {
                if (device.TryGetFeatureValue(CommonUsages.centerEyePosition, out Vector3 position))
                {
                    transform.localPosition = position;
                }
                
                if (device.TryGetFeatureValue(CommonUsages.centerEyeRotation, out Quaternion rotation))
                {
                    transform.localRotation = rotation;
                }
            }
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
