using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JSONRequest : MonoBehaviour
{
    public Text testText;
    string url = "https://client.dev.kote.robotseamonster.com/TEST_HARNESS/json_files/cards.json";

    void Start()
    {
        GetData();
    }

    void GetData() => StartCoroutine(GetDataCoroutine());

    IEnumerator GetDataCoroutine()
    {
        using(UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                testText.text = request.error;
            }
            else
            {
                testText.text = request.downloadHandler.text;
            }
        }
    }
}
