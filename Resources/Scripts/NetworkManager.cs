using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace EDVR.HybirdClassoom
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public Button createRoomButton;
        public GameObject loadingPanel;
        public GameObject nameUI;
        public GameObject recordingUiItemPrefab;
        public Transform recordingUiItemParent;
        public TextMeshProUGUI loadtext;
        public TMP_InputField nameInputField;

        private bool noRoomFound;
        private bool joinPressed;
        private bool directPlay;
        private bool triesToConnectToMaster = false;
        private string savedName;

        private string directPlayScene;

        private void Start()
        {
            if (createRoomButton)
            {
                createRoomButton.onClick.AddListener(CreateRoom);
            }

            if (PlayerPrefs.HasKey("DirectPlay"))
            {
                directPlay = PlayerPrefs.GetString("DirectPlay") != "";
                if (directPlay)
                {
                    directPlayScene = PlayerPrefs.GetString("DirectPlay");
                    PlayerPrefs.SetString("DirectPlay", "");
                }
            }

            ToggleLoading(!PhotonNetwork.IsConnected, "Connecting...");

            if (PlayerPrefs.HasKey("LocalPlayerName"))
            {
                savedName = PlayerPrefs.GetString("LocalPlayerName");
                nameInputField.text = savedName;
            }

            if (recordingUiItemParent != null)
            {
                foreach (var item in SaveSystem.GetRecordings())
                {
                    GameObject go = Instantiate(recordingUiItemPrefab, recordingUiItemParent);
                    go.GetComponent<RecordingUiItem>().Set(item, this);
                }
            }
        }

        public void EnterName()
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

        public void ToggleLoading(bool isOn, string _loadingtxt)
        {
            if (loadingPanel)
            {
                loadingPanel.SetActive(isOn);
            }

            if (loadtext)
            {
                loadtext.text = _loadingtxt;
            }
        }

        private void Update()
        {
            if (!PhotonNetwork.IsConnected && !triesToConnectToMaster)
            {
                ConnectToMaster();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                JoinRandomRoom();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && !PhotonNetwork.IsConnected)
            {
                triesToConnectToMaster = false;
            }
        }

        private void ToggleCreateRoomButton(bool isOn)
        {
            if (createRoomButton)
            {
                createRoomButton.interactable = isOn;
            }
        }

        private void JoinRandomRoom()
        {
            if (joinPressed)
            {
                return;
            }

            joinPressed = true;

            if (!PhotonNetwork.IsConnectedAndReady)
            {
                return;
            }

            Debug.Log("Joining random room");

            ToggleLoading(true, "Searching for rooms...");

            PhotonNetwork.JoinRandomRoom();
        }

        public void ConnectToMaster()
        {
            PhotonNetwork.OfflineMode = false; 
            PhotonNetwork.NickName = "PlayerName"; 
            PhotonNetwork.AutomaticallySyncScene = true; 
            PhotonNetwork.GameVersion = "v1"; 

            triesToConnectToMaster = true;
            PhotonNetwork.ConnectUsingSettings();

            ToggleCreateRoomButton(false);

            Debug.Log("Connecting...");

            ToggleLoading(true, "Connecting...");
        }  
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            triesToConnectToMaster = false;
            Debug.Log(cause);

            joinPressed = false;

            ToggleCreateRoomButton(false);

            ToggleLoading(true, "Connecting...");
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            triesToConnectToMaster = false;
            Debug.Log("Connected to master");

            ToggleCreateRoomButton(true);

            if (directPlay)
            {
                joinPressed = false;
                CreateRoom();
            }
            else if (joinPressed)
            {
                JoinRandomRoom();
            }

            ToggleLoading(false, "");
        }

        private void CreateRoom()
        {
            if (!PhotonNetwork.IsConnectedAndReady || joinPressed)
            {
                return;
            }

            Debug.Log("Creating Room");

            joinPressed = true;

            ToggleCreateRoomButton(false);
            string roomName = Random.Range(10000, 100000).ToString();
            PhotonNetwork.CreateRoom(roomName, null);

            string txt = noRoomFound ? "No room found. Creating new room" : "Creating room...";

            noRoomFound = false;

            ToggleLoading(true, txt);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);

            joinPressed = false;

            Debug.Log(message);

            CreateRoom();
        }

        public override void OnJoinedRoom()
        {
            //Go to next scene after joining the room
            base.OnJoinedRoom();
            Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name + " Region: " + PhotonNetwork.CloudRegion);

            ToggleLoading(true, "Joining room: " + PhotonNetwork.CurrentRoom.Name);

            SceneManager.LoadSceneAsync(directPlay ? directPlayScene : "Classroom"); //go to the room scene
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);

            Debug.Log(message);

            noRoomFound = true;

            joinPressed = false;

            CreateRoom();
        }

        public void PlayRecording(string recordingName)
        {
            PlayerPrefs.SetString("CurrentRecording", recordingName);
            SceneManager.LoadScene("RecordingScene");
        }
    }
}