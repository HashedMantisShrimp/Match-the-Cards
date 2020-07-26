using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public static class Internet
{
    internal static async Task<bool> CheckInternetConnectivity()//Checks Internet connection by pinging Google
    {
        try
        {
            UnityWebRequest newRequest = new UnityWebRequest("https://google.com");
            newRequest.timeout = 3;

            newRequest.SendWebRequest();

            while (!newRequest.isDone)
            {
                await Task.Yield();
            }

            if (newRequest.isNetworkError)
                Debug.Log($"<color=red>Error</color> found while checking connection: {newRequest.error}.");

            Debug.Log($"{nameof(Internet)} connection: {!newRequest.isNetworkError}");
            return !newRequest.isNetworkError;
        }
        catch (Exception e)
        {
            Misc.HandleException(e, GameData.GetExcInternetConnectivity());
        }

        Debug.Log($"<color=red>ERROR:</color> Out of InternetConnectivity() Try-Catch block, returning false");
        return false;
    }
}
