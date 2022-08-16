using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using EDVR.HybirdClassoom;
using System.Collections.Generic;
using UnityEngine.XR;

public class BoardSelection : MonoBehaviourPunCallbacks
{
    public static BoardSelection Instance;

    public GameObject[] whiteboard;
    public GameObject[] slides;
    public TMP_Dropdown boardDropdown;

    private int initialBoardIndex;

    private bool recording;

    //private Dictionary<float, int> recordList = new Dictionary<float, int>();
    private IndexRecordData recordData;

    private int currentIndex;

    private bool initialized;

    public GameObject changeModeButton;

    private SimpleSlides slideManager;

    private GameObject previewSlide;

    private InputDevice targetDevice;

    private bool down;

    private void Awake()
    {
        if (Instance)
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        slideManager = FindObjectOfType<SimpleSlides>();

        if (!initialized)
        {
            RpcSet(boardDropdown.value);
        }

        boardDropdown.onValueChanged.AddListener(Set);

        initialBoardIndex = boardDropdown.value;

        UiEvents.Instance.OnRecord += Record;

        changeModeButton.SetActive(false);
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

        recordData = new IndexRecordData();

        if (recording)
        {
            recordData.Add(currentIndex);
        }
        else
        {
            SaveRecording();
        }
    }

    private void SaveRecording()
    {
        SaveSystem.SavePlayer(ref recordData, UID.BoardSelection);
    }

    public void Set(int index)
    {
        photonView.RPC("RpcSet", RpcTarget.All, index);
    }

    [PunRPC]
    private void RpcSet(int index)
    {
        bool _whiteboard = false;
        bool _slides = false;

        changeModeButton.SetActive(_slides);

        if (!_slides)
        {
            slideManager.SwitchModeReset();
        }

        switch (index)
        {
            case 1:
                _whiteboard = true;
                break;
            case 2:
                _slides = true;
                break;
        }

        foreach (var item in whiteboard)
        {
            item.SetActive(_whiteboard);
        }

        foreach (var item in slides)
        {
            if (item.CompareTag("TeacherScreen"))
            {
                previewSlide = item;
                item.SetActive(_slides && PlatformManager.instance.mode == PlatformManager.Mode.Teacher);
            }
            else
            {
                item.SetActive(_slides);
            }
        }

        if (recording)
        {
            recordData.Add(index);
        }

        currentIndex = index;

        initialized = true;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient && boardDropdown.value != initialBoardIndex)
        {
            photonView.RPC("RpcSet", newPlayer, boardDropdown.value);
        }
    }

    private void SetDevice()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }

    private void Update()
    {
        if (!targetDevice.isValid)
        {
            SetDevice();
            return;
        }

        if (currentIndex == 2)
        {
            targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton);

            if (primaryButton && !down)
            {
                previewSlide.SetActive(!previewSlide.activeInHierarchy);
                down = true;
            }

            if (!primaryButton && down)
            {
                down = false;
            }
        }
    }
}
