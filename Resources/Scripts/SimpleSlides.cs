using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using EDVR.HybirdClassoom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Paroxe.PdfRenderer;
using Paroxe.PdfRenderer.WebGL;
using Newtonsoft.Json;
using Photon.Realtime;

[System.Serializable]
public class Datum
{
    public string filename { get; set; }
    public string URL { get; set; }
}

[System.Serializable]
public class Root
{
    public List<Datum> data { get; set; }
}

public class SimpleSlides : MonoBehaviourPunCallbacks
{
    public int PageIndex { get; private set; }
    public bool CanvasMode { get; private set; }
    public int PdfIndex { get; private set; }

    public TMP_Dropdown Docoptions;
    public MeshRenderer[] displays;
    public Image canvasScreen;
    public TextMeshProUGUI screenModeText;
    public GameObject dropDown;
    public GameObject[] loading;
    public bool test;
    public Texture2D[] testSlides;

    private VRPointer vrpointer;
    private bool freeze;
    private bool initialization;
    private bool lateJoin;
    private bool recording;
    private string jsonurl = "https://drive.google.com/uc?export=download&id=1X0lPf6nqJ0hL5BHAj4Bl7HfIAUFZNJpP";
    private List<Texture2D> textures = new List<Texture2D>();
    //private Dictionary<float, Texture2D> pdfRecordList = new Dictionary<float, Texture2D>();
    private PdfRecordData recordData;
    private Root root;
    private Texture2D currentTexture;

    private void Start()
    {
        if (VRPointer.Instance)
        {
            vrpointer = VRPointer.Instance;
        }

        screenModeText.text = "3D";

        if (!test)
        {
            PdfIndex = 1000;
            StartCoroutine(GetData());
        }
        else if (!lateJoin)
        {
            RpcDisplaySlide(0);
        }

        ToggleLoading(false);

        UiEvents.Instance.OnRecord += Record;

        recordData = new PdfRecordData();
    }

    private void OnDestroy()
    {
        if (recording)
        {
            SaveRecording();
        }

        UiEvents.Instance.OnRecord -= Record;
    }

    private void Record(bool isRecording)
    {
        recording = isRecording;

        if (recording)
        {
            if (currentTexture != null)
            {
                recordData.Add(currentTexture.EncodeToPNG());
            }
        }
        else
        {
            SaveRecording();
        }
    }

    private void SaveRecording()
    {
        SaveSystem.SavePlayer(ref recordData, UID.SlidesData);
        recordData = new PdfRecordData();
    }

    private void ToggleLoading(bool isOn)
    {
        foreach (var item in loading)
        {
            item.SetActive(isOn);
        }
    }

    private IEnumerator GetData()
    {
        UnityWebRequest request = UnityWebRequest.Get(jsonurl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            //Error Bitch
        }
        else
        {
            while (!request.isDone)
            {
                yield return null;
            }
            root = JsonConvert.DeserializeObject<Root>(request.downloadHandler.text);
            List<string> dropdownoptions = new List<string>();
            foreach (var item in root.data)
            {
                dropdownoptions.Add(item.filename);
            }
            Docoptions.AddOptions(dropdownoptions);

            initialization = true;

            if (lateJoin)
            {
                RpcDownloadPdf(PdfIndex);
            }
        }
    }

    public void DownloadPDF()
    {
        if (Docoptions.value > 0)
        {
            ToggleLoading(true);
            photonView.RPC("RpcDownloadPdf", RpcTarget.All, Docoptions.value - 1);
        }
    }

    [PunRPC]
    private void RpcDownloadPdf(int index)
    {
        if (index >= root.data.Count)
        {
            return;
        }

        ToggleLoading(true);
        StartCoroutine(GetPDFFile(root.data[index].URL));
        PdfIndex = index;
    }

    private IEnumerator GetPDFFile(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Access-Control-Allow-Origin", "*");
        request.SetRequestHeader("Method", "GET");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            //Error Bitch
        }
        else
        {
            byte[] bytes = request.downloadHandler.data;
            StartCoroutine(MakePDF(bytes));
        }
        request.Dispose();
    }


    private IEnumerator MakePDF(byte[] bytes)
    {
        PDFJS_Promise<PDFDocument> documentPromise = PDFDocument.LoadDocumentFromBytesAsync(bytes);

        while (!documentPromise.HasFinished)
            yield return null;

        if (!documentPromise.HasSucceeded)
        {
            Debug.Log("Fail: documentPromise");

            yield break;
        }

        Debug.Log("Success: documentPromise");

        PDFDocument pdfdocument = documentPromise.Result;



        for (int i = 0; i < pdfdocument.GetPageCount(); i++)
        {
            PDFJS_Promise<PDFPage> pagePromise = pdfdocument.GetPageAsync(i);
            while (!pagePromise.HasFinished)
                yield return null;

            if (!pagePromise.HasSucceeded)
            {
                Debug.Log("Fail: pagePromise");

                yield break;
            }

            Debug.Log("Success: pagePromise");

            PDFPage page = pagePromise.Result;

            PDFJS_Promise<Texture2D> renderPromise = PDFRenderer.RenderPageToTextureAsync(page, (int)page.GetPageSize().x, (int)page.GetPageSize().y);
            textures.Add(renderPromise.Result);
        }



        ToggleLoading(false);

        RpcDisplaySlide(PageIndex);

    }

    private void Update()
    {
        if (PlatformManager.instance.mode != PlatformManager.Mode.Teacher || freeze || BoardSelection.Instance.boardDropdown.value != 2 || (textures.Count == 0 && !test))
        {
            return;
        }

        if (vrpointer.RightJoyStick)
        {
            Next();
        }

        if (vrpointer.LeftJoyStick)
        {
            Previous();
        }
    }

    [PunRPC]
    private void RpcDisplaySlide(int index)
    {
        PageIndex = index;

        currentTexture = test ? testSlides[index] : textures[index];

        foreach (var item in displays)
        {
            item.material.mainTexture = currentTexture;
        }

        Rect rec = new Rect(0, 0, currentTexture.width, currentTexture.height);
        canvasScreen.sprite = Sprite.Create(currentTexture, rec, new Vector2(0.5f, 0.5f), 100);

        canvasScreen.enabled = !canvasScreen.enabled;
        canvasScreen.enabled = !canvasScreen.enabled;

        Invoke("UnFreeze", 0.5f);

        if (recording)
        {
            recordData.Add(currentTexture.EncodeToPNG());
        }
    }

    private void UnFreeze()
    {
        freeze = false;
    }

    public void Next()
    {
        int totalPages = test ? testSlides.Length : textures.Count;
        if (PageIndex == totalPages - 1)
        {
            return;
        }

        freeze = true;
        PageIndex++;

        photonView.RPC("RpcDisplaySlide", RpcTarget.All, PageIndex);
    }

    public void Previous()
    {
        if (PageIndex == 0)
        {
            return;
        }

        freeze = true;
        PageIndex--;

        photonView.RPC("RpcDisplaySlide", RpcTarget.All, PageIndex);
    }

    public void SwitchMode()
    {
        CanvasMode = !CanvasMode;

        FindObjectOfType<CameraMover>().ToggleCanvasMode(CanvasMode);

        screenModeText.text = CanvasMode ? "2D" : "3D";

        if (CanvasMode)
        {
            PlatformManager.instance.ResetFov();
        }
        else
        {
            canvasScreen.enabled = false;
        }
    }

    public void SwitchModeReset()
    {
        if (!CanvasMode)
        {
            return;
        }

        CanvasMode = false;

        FindObjectOfType<CameraMover>().ToggleCanvasMode(CanvasMode);

        screenModeText.text = CanvasMode ? "2D" : "3D";

        if (CanvasMode)
        {
            PlatformManager.instance.ResetFov();
        }
        else
        {
            canvasScreen.enabled = false;
        }
    }

    public void ToggleCanvas(bool value)
    {
        canvasScreen.enabled = value;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (initialization && PhotonNetwork.IsMasterClient && ((PdfIndex < root.data.Count && !test) || (test && PdfIndex != 0)))
        {
            photonView.RPC("LateJoinInitialize", newPlayer, PdfIndex, PageIndex);
        }
    }

    [PunRPC]
    private void LateJoinInitialize(int activePdfIndex, int activePageIndex)
    {
        lateJoin = true;

        PageIndex = activePageIndex;

        if (initialization)
        {
            RpcDownloadPdf(activePdfIndex);
        }
        else
        {
            PdfIndex = activePdfIndex;
        }

        if (!PhotonNetwork.IsMasterClient && PlatformManager.instance.mode == PlatformManager.Mode.Teacher)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }
    }
}
