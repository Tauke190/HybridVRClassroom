using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using EDVR.HybirdClassoom;

public class LobbyStudentPC : MonoBehaviourPunCallbacks
{
    public GameObject loadingPanel;
    public GameObject nameUI;
    public TMP_InputField roomInputField;

    private GameObject joinUI;
    private TMP_InputField nameInputField;

    private string savedName;

    private void Start()
    {
        nameUI = GetComponent<NetworkManager>().nameUI;
        nameInputField = GetComponent<NetworkManager>().nameInputField;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (loadingPanel.activeInHierarchy)
            {
                //do nothing
            }
            else if (nameUI.activeInHierarchy)
            {
                EnterName();
            }          
            else if (joinUI.activeInHierarchy)
            {
                JoinRoom();
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public void JoinRoom()
    {
        if (roomInputField.text != "")
        {
            PhotonNetwork.JoinRoom(roomInputField.text);
            GetComponent<NetworkManager>().ToggleLoading(true, "Joining room " + roomInputField.text + "...");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        roomInputField.text = "";

        base.OnJoinRoomFailed(returnCode, message);
    }

    private void EnterName()
    {
        if (nameInputField.text != "")
        {
            if (savedName != nameInputField.text)
            {
                savedName = nameInputField.text;
                PlayerPrefs.SetString("LocalPlayerName", savedName);
            }
            nameUI.SetActive(false);
            PhotonNetwork.LocalPlayer.NickName = savedName;
        }
    }
}
