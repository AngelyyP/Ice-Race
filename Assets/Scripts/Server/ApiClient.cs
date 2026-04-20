using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    public string baseUrl = "http://127.0.0.1:5000/server";

    public event Action<int , ServerData> OnDataReceived;

    public IEnumerator IsPlayerSlotOccupied(string gameId, string playerId, Action<bool> onComplete)
    {
        string url = $"{baseUrl}/{gameId}/{playerId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            bool occupied = webRequest.responseCode == 200;
            onComplete?.Invoke(occupied);
        }
    }

    public IEnumerator GetPlayerData(string gameId, string playerId, Action onFinished = null)
    {
        string url = $"{baseUrl}/{gameId}/{playerId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                if (webRequest.responseCode == 404)
                {

                }
                else
                {
                    Debug.LogError($"GET Error: {webRequest.error}");
                    Debug.LogError($"Response: {webRequest.downloadHandler.text}");
                }
            }
            else
            {

                var data = JsonUtility.FromJson<ServerData>(webRequest.downloadHandler.text);
                OnDataReceived?.Invoke( Convert.ToInt16(playerId), data);
            }
        }
        
        onFinished?.Invoke();
    }

    public IEnumerator PostPlayerData(string gameId, string playerId, ServerData data, Action onFinished = null)
    {
        string url = $"{baseUrl}/{gameId}/{playerId}";
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"POST Error: {webRequest.error}");
                Debug.LogError($"Response: {webRequest.downloadHandler.text}");
            }
            else
            {

            }
        }
        
        onFinished?.Invoke();
    }
}
