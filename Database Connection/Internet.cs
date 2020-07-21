using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public static class Internet
{
    
    internal static async Task<bool> CheckInternetConnectivity()//Checks Internet connection by pinging Google
    {
        UnityWebRequest newRequest = new UnityWebRequest("https://google.com");
        newRequest.timeout = 2;

        newRequest.SendWebRequest();

        while (!newRequest.isDone)
        {
           await Task.Yield();
        }

        if (newRequest.isNetworkError)
            Debug.Log($"<color=red>Error</color> found while checking connection: {newRequest.error}.");

        Debug.Log($"{nameof(newRequest)} connection: {!newRequest.isNetworkError}");
        return !newRequest.isNetworkError;
    }
}
