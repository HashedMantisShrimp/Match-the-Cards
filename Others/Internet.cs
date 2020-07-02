using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class Internet
{
    internal static IEnumerator CheckInternetConnections(System.Action<bool> isConnected)//Checks internet connection status
    {
        UnityWebRequest newRequest = new UnityWebRequest("https://google.com");
        newRequest.timeout = 2;

        yield return newRequest.SendWebRequest();
        
        if (newRequest.isNetworkError)
        {
            Debug.Log($"<color=red>Error msg</color>: {newRequest.error}");
            isConnected(false);
        }
        else
        {
            isConnected(true);
        }
    }
}
