using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EDVR.HybirdClassoom;

public class RecordingUiItem : MonoBehaviour
{
    private string recordingName;

    private NetworkManager networkManager;

    public void Set(string _recordingName, NetworkManager _networkManager)
    {
        recordingName = _recordingName;
        GetComponentInChildren<TextMeshProUGUI>().text = recordingName;
        GetComponent<Button>().onClick.AddListener(OnClick);
        networkManager = _networkManager;
    }

    private void OnClick()
    {
        networkManager.PlayRecording(recordingName);
    }
}
