using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using EDVR.HybirdClassoom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class ReJoinHandler : MonoBehaviourPunCallbacks
{
    public static ReJoinHandler Instance { get; private set; }

    private string lastRoomName;
    private Button rejoinbutton;
    private bool rejoinEnable;

    private void Awake()
    {
        if (Instance)
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            return;
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        if (lastRoomName == "" || SceneManager.GetActiveScene().name != "LobbyVR")
        {
            return;
        }

        RoomInfo lastRoomInfo = roomList.FirstOrDefault(r => r.Name == lastRoomName);

        if (!rejoinbutton)
        {
            rejoinbutton = GameObject.FindGameObjectWithTag("ReJoinButton").GetComponent<Button>();
        }

        if (lastRoomInfo == null)
        {
            rejoinbutton.interactable = false;
            rejoinEnable = false;
            lastRoomName = "";
        }
        else
        {
            rejoinbutton.interactable = true;

            if (!rejoinEnable)
            {
                rejoinbutton.onClick.AddListener(ReJoin);
                rejoinEnable = true;
            }
        }
    }

    private void ReJoin()
    {
        rejoinbutton.interactable = false;
        GameObject.FindGameObjectWithTag("CreateButton").GetComponent<Button>().interactable = false;
        PhotonNetwork.JoinRoom(lastRoomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        GameObject.FindGameObjectWithTag("CreateButton").GetComponent<Button>().interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        if (SceneManager.GetActiveScene().name == "Classroom" && PlatformManager.instance.mode == PlatformManager.Mode.Teacher)
        {
            lastRoomName = PlatformManager.instance.RoomName;
        }
    }
}
