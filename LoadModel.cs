using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
//using Siccity.GLTFUtility;

public class LoadModel : MonoBehaviour
{

    [SerializeField]
    private string weburl = "https://drive.google.com/uc?export=download&id=1Ro_m3l5H99mhitgC-ZGMvTlwjTtzdurD";

    [SerializeField]
    private GameObject modelParent;

    private string filepath;
 



    //private void Start()
    //{

    //    filepath = $"{Application.persistentDataPath}/Files/test.gltf";
    //    modelParent = new GameObject
    //    {
    //        name = "Model"
    //    };
    //    DownloadFilefromtheWeb();
    //}


    //private void DownloadFilefromtheWeb()
    //{
    //    StartCoroutine(DownloadFile(weburl));
    //}


    //private IEnumerator DownloadFile(string url)
    //{
    //    UnityWebRequest request = UnityWebRequest.Get(url);
    //    yield return request.SendWebRequest();


    //    if (request.result == UnityWebRequest.Result.ConnectionError)
    //    {
    //        Debug.Log("Error while downlaoding 3D model");
    //    }
    //    else
    //    {
    //        Debug.Log(request.downloadHandler.data);

    //        byte[]  modeldata = request.downloadHandler.data;

    //        GameObject model = Importer.LoadFromBytes(modeldata);
            
    //        model.transform.SetParent(modelParent.transform);
            
            
    //    }
    //    request.Dispose();
    //}


}
