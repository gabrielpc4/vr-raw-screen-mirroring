using UnityEngine;

public class MulticastLock : MonoBehaviour
{
    private AndroidJavaObject multicastLock;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    using (AndroidJavaObject wifiManager = context.Call<AndroidJavaObject>("getSystemService", "wifi"))
                    {
                        multicastLock = wifiManager.Call<AndroidJavaObject>("createMulticastLock", "NDILock");
                        multicastLock.Call("setReferenceCounted", true);
                        multicastLock.Call("acquire");
                        Debug.Log("Multicast Lock acquired for NDI discovery.");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to acquire Multicast Lock: " + e.Message);
        }
#endif
    }

    void OnDestroy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (multicastLock != null)
        {
            multicastLock.Call("release");
            multicastLock = null;
            Debug.Log("Multicast Lock released.");
        }
#endif
    }
}
