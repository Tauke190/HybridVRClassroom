using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using UnityEngine.UI;
using Byn.Unity.Examples;

namespace EDVR.HybirdClassoom {
    public class PlatformManager : MonoBehaviourPunCallbacks {
        [SerializeField] GameObject teacherRig;
        public GameObject studentRig;
        public Transform[] studentPositions;

        public Avatar avatar;
        private FollowVRRig teacherBodyFollow;
        [SerializeField] GameObject[] teacherAvatars;

        public Transform[] teacherRigParts;
        public Transform[] studentRigParts;

        [SerializeField] GameObject teacherBody;
        [SerializeField] GameObject studentBody;
        [SerializeField] GameObject studentBodyNonVR;

        [SerializeField] GameObject teacherSpecificTools;
        [SerializeField] private CallAppUi callAppUi;
        [SerializeField] private ConferenceApp conferenceApp;

        //Seats
        Hashtable h = new Hashtable();
        bool initialized;
        bool seated;
        int actorNum;

        //Modes
        public enum Mode { Teacher, NONO, StudentPC };
        [Tooltip("Choose the mode before building")]
        public Mode mode;

        //Whiteboards
        public Whiteboard smallWhiteboard;

        public static PlatformManager instance;

        private float initialFov;

        private float targetFov;

        public GameObject nonVR;

        public GameObject vrinputmodule;
        public GameObject standaloneinputmodule;

        public GameObject StudentPhoneBody { get; private set; }

        public Transform screen;

        public string RoomName { get; private set; }

        public GameObject LocalPlayerObject { get; private set; }

        private Transform lookPoint;

        private List<SeatSelectButton> seatButtons = new List<SeatSelectButton>();

        public GameObject seatPanel;

        private bool returning;

        public GameObject vrui;

        public GameObject invitationPanel;

        public Button hostPlayerReturnButton;

        public GameObject returnToSeatButton;

        public Transform stageSpawnPoint;

        private int myChairPosition;

        public bool PlayerOnStage { get; private set; }

        private StudentBodyFollow stagePlayer;

        public bool OnStage { get; private set; }

        void Awake()
        {
            //If not connected go to lobby to connect
            if (!PhotonNetwork.IsConnected) {
#if UNITY_EDITOR
                PlayerPrefs.SetString("DirectPlay", SceneManager.GetActiveScene().name);
#endif
                SceneManager.LoadScene(0);
                return;
            }

            PlayerPrefs.SetInt("DirectPlay", 0);

            instance = this;
            actorNum = PhotonNetwork.LocalPlayer.ActorNumber;

            //If student connecting from phone, limit the fps to save battery. Also avoid sleep.
            //if (mode == Mode.StudentPhone) {
            //    QualitySettings.vSyncCount = 0;
            //    Application.targetFrameRate = 30;
            //    Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //}

            lookPoint = GameObject.FindGameObjectWithTag("LookPoint").transform;

            PlayerPrefs.SetString("RoomName", PhotonNetwork.CurrentRoom.Name);
            PlayerPrefs.SetInt("Video", mode == Mode.StudentPC ? 1 : 0);
        }

        public void Start()
        {
            vrui.SetActive(mode == Mode.Teacher);

            if (!PhotonNetwork.IsConnected)
            {
                return;
            }

            //if this is the first player to connect, initialize the students list
            if (PhotonNetwork.IsMasterClient && !initialized) {
                InitializeStudentList();
            }

            hostPlayerReturnButton.interactable = false;
            returnToSeatButton.SetActive(false);

            nonVR.SetActive(mode == Mode.StudentPC);
            seatPanel.SetActive(true);

            seatButtons = FindObjectsOfType<SeatSelectButton>().ToList();

            seatPanel.SetActive(false);

            teacherRig.SetActive(mode == Mode.Teacher);
            studentRig.SetActive(mode == Mode.StudentPC);

            //if this is the teacher, activate its rig and create the body
            if (mode == Mode.Teacher) {
                CreateTeacherBody();
                smallWhiteboard.GetComponent<PhotonView>().RequestOwnership();
                teacherSpecificTools.SetActive(true);
                // callAppUi.ModifiedSetupCallApp(true,false,"Machikne");          
            }
            //if it's a student, create it's body and sit in right position if the student list already exists
            else if (mode == Mode.NONO || mode == Mode.StudentPC) {
                CreateStudentBody();
                if (PhotonNetwork.CurrentRoom.CustomProperties["Initialized"] != null) {
                    Sit(GetFreeSeat(), 0);
                }

                initialFov = studentRigParts[0].GetComponent<Camera>().fieldOfView;
                targetFov = initialFov;
                //callAppUi.ModifiedSetupCallApp(true, true, "Machikne");
            }

            UiEvents.Instance.OnZoom += FovToggle;

            vrinputmodule.SetActive(mode == Mode.Teacher);
            standaloneinputmodule.SetActive(mode == Mode.StudentPC);

            RoomName = PhotonNetwork.CurrentRoom.Name;
        }
       
        private void OnDestroy()
        {
            UiEvents.Instance.OnZoom -= FovToggle;
        }

        private void FovToggle()
        {
            targetFov = targetFov == initialFov ? initialFov / 2 : initialFov;
        }

        public void ResetFov()
        {
            targetFov = initialFov;
        }

        private void Update()
        {
            if (!PhotonNetwork.IsConnected && !returning)
            {
                GoToScene(0);
                return;
            }

            if (mode == Mode.StudentPC)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    FovToggle();
                }

                float currentFov = studentRigParts[0].GetComponent<Camera>().fieldOfView;

                if (Mathf.Abs(targetFov - currentFov) < 2f)
                {
                    studentRigParts[0].GetComponent<Camera>().fieldOfView = targetFov;
                }
                else
                {
                    if (targetFov > currentFov)
                    {
                        studentRigParts[0].GetComponent<Camera>().fieldOfView += Time.deltaTime * 100;
                    }

                    if (targetFov < currentFov)
                    {
                        studentRigParts[0].GetComponent<Camera>().fieldOfView -= Time.deltaTime * 100;
                    }
                }
            }
        }

        void CreateTeacherBody() {
            teacherBodyFollow = PhotonNetwork.Instantiate(teacherBody.name, transform.position, transform.rotation).GetComponent<FollowVRRig>();
            foreach (var item in teacherAvatars) {
                item.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
            LocalPlayerObject = teacherBodyFollow.gameObject;
        }

        public void SetMaleAvatar() {
            photonView.RPC("ChangeTeacherAvatar", RpcTarget.AllBuffered, "male");
            teacherBodyFollow.GetComponent<PhotonView>().RPC("SetAvatarFollow", RpcTarget.AllBuffered);
        }

        public void SetFemaleAvatar() {
            photonView.RPC("ChangeTeacherAvatar", RpcTarget.AllBuffered, "female");
            teacherBodyFollow.GetComponent<PhotonView>().RPC("SetAvatarFollow", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void ChangeTeacherAvatar(string gender) {
            if (gender == "female") {
                teacherAvatars[0].SetActive(false);
                teacherAvatars[1].SetActive(true);
                avatar = teacherAvatars[1].GetComponent<Avatar>();
            } else if (gender == "male") {
                teacherAvatars[1].SetActive(false);
                teacherAvatars[0].SetActive(true);
                avatar = teacherAvatars[0].GetComponent<Avatar>();
            }
        }

        void CreateStudentBody() {
            if (mode == Mode.NONO) {
                LocalPlayerObject = PhotonNetwork.Instantiate(studentBody.name, transform.position, transform.rotation);
            } else if (mode == Mode.StudentPC) {
                StudentPhoneBody = PhotonNetwork.Instantiate(studentBodyNonVR.name, transform.position, transform.rotation);
                LocalPlayerObject = StudentPhoneBody;
            }
        }


        //So we stop loading scenes if we quit app
        private void OnApplicationQuit() {
            StopAllCoroutines();
        }

        //This creates an empty list of students matching the number of seats
        void InitializeStudentList() {
            if (PhotonNetwork.CurrentRoom.CustomProperties["Initialized"] == null) {
                h.Add("Initialized", actorNum);
                for (int i = 0; i < studentPositions.Length; i++) {
                    h.Add("" + i, 0);
                }
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(h);
        }

        //Gets the first sit that is free (that has a value of 0)
        int GetFreeSeat() {
            for (int i = 0; i < studentPositions.Length; i++) {
                if ((int)PhotonNetwork.CurrentRoom.CustomProperties["" + i] == 0) {
                    return i;
                }
            }
            return -1;
        }

        //Puts the student in the correspondant desk
        void Sit(int n, int move) {
            if (n == -1) {
                Debug.LogError("No sits available");
            }
            Debug.Log("Sitting student in seat " + n);
            if (mode == Mode.NONO) {
                studentRig.transform.position = studentPositions[n].position + (studentPositions[n].forward * 0.4f) - (studentPositions[n].up * 0.3f);
            } else if (mode == Mode.StudentPC) {
                studentRig.transform.position = studentPositions[n].position + (studentPositions[n].up * 0.5f) + (studentPositions[n].forward * 0.4f);
            }
            studentRig.transform.forward = (lookPoint.position - studentRig.transform.position).normalized;
            studentRig.transform.eulerAngles = new Vector3(0, studentRig.transform.eulerAngles.y, 0);
            seated = true;
            h["" + n] = actorNum;
            PhotonNetwork.CurrentRoom.SetCustomProperties(h);

            Debug.Log(studentRig.transform.position);
            Debug.Log(studentPositions[n].position);

            StudentPhoneBody.GetComponent<StudentBodyFollow>().SyncSeat(n, move);

            FindObjectOfType<CameraMover>().InitializePosition();

            myChairPosition = n;
        }

        public void GoToStage()
        {
            studentRig.transform.position = lookPoint.position;
            studentRig.transform.rotation = lookPoint.rotation;

            StudentPhoneBody.GetComponent<StudentBodyFollow>().GoToStage();

            returnToSeatButton.SetActive(true);

            OnStage = true;

            foreach (var item in FindObjectsOfType<BoneRef>())
            {
                item.Freeze(false);
            }
        }

        public void ReturnToSeat()
        {
            studentRig.transform.position = studentPositions[myChairPosition].position + (studentPositions[myChairPosition].up * 0.5f) + (studentPositions[myChairPosition].forward * 0.4f);
            studentRig.transform.forward = (lookPoint.position - studentRig.transform.position).normalized;
            studentRig.transform.eulerAngles = new Vector3(0, studentRig.transform.eulerAngles.y, 0);

            returnToSeatButton.SetActive(false);

            StudentPhoneBody.GetComponent<StudentBodyFollow>().SyncSeat(myChairPosition, -1);

            OnStage = false;

            foreach (var item in FindObjectsOfType<BoneRef>())
            {
                item.Freeze(true);
            }
        }

        public void OnPlayerEnterStage(StudentBodyFollow _stagePlayer)
        {
            stagePlayer = _stagePlayer;

            PlayerOnStage = true;

            hostPlayerReturnButton.interactable = true;

            VRPointer.Instance.DropBone();

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var item in FindObjectsOfType<BoneRef>())
                {
                    item.Freeze(true);
                }
            }
        }

        public void OnPlayerLeftStage()
        {
            PlayerOnStage = false;

            hostPlayerReturnButton.interactable = false;

            stagePlayer = null;

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var item in FindObjectsOfType<BoneRef>())
                {
                    item.Freeze(false);
                }
            }
        }

        public void HostReturnPlayerToSeat()
        {
            photonView.RPC("RpcReturnPlayerByHost", RpcTarget.All);

            foreach (var item in FindObjectsOfType<BoneRef>())
            {
                item.Freeze(false);
            }
        }

        [PunRPC]
        private void RpcReturnPlayerByHost()
        {
            if (stagePlayer)
            {
                Transform chair = studentPositions[stagePlayer.ChairIndex];

                stagePlayer.body.transform.SetParent(chair);

                stagePlayer.body.transform.localPosition = new Vector3(0, 0.5f, 0.1f);
                stagePlayer.body.transform.localEulerAngles = new Vector3(-90, 0, 0);

                stagePlayer.body.transform.SetParent(stagePlayer.transform);

                stagePlayer = null;
            }
            else if (OnStage)
            {
                studentRig.transform.position = studentPositions[myChairPosition].position + (studentPositions[myChairPosition].up * 0.5f) + (studentPositions[myChairPosition].forward * 0.4f);
                studentRig.transform.forward = (lookPoint.position - studentRig.transform.position).normalized;
                studentRig.transform.eulerAngles = new Vector3(0, studentRig.transform.eulerAngles.y, 0);

                returnToSeatButton.SetActive(false);

                OnStage = false;

                foreach (var item in FindObjectsOfType<BoneRef>())
                {
                    item.Freeze(true);
                }
            }
        }

        //This is called when the room properties are updated, for example, when the student list is created
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            if (propertiesThatChanged.ContainsKey("Initialized") && !initialized)
            {
                Debug.Log("Student list initialized");
                initialized = true;
                for (int i = 0; i < studentPositions.Length; i++)
                {
                    Debug.Log((int)propertiesThatChanged["" + i]);
                }
                if ((mode == Mode.NONO || mode == Mode.StudentPC) && !seated)
                {
                    Sit(GetFreeSeat(), 0);
                }
            }

            for (int i = 0; i < studentPositions.Length; i++)
            {
                SeatSelectButton buton = seatButtons.FirstOrDefault(b => b.id == studentPositions[i].GetComponent<ChairRef>().id);
                if (buton != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("" + i))
                {
                    buton.GetComponent<Button>().interactable = (int)PhotonNetwork.CurrentRoom.CustomProperties["" + i] == 0;
                }
            }
        }

        public void ChangeSeat(string newSeatId)
        {
            seatPanel.SetActive(false);

            FindObjectOfType<CameraMover>().ResetOnSeatChanged();

            bool removed = false;
            bool seatfound = false;

            int newSeatIndex = -1;

            for (int i = 0; i < studentPositions.Length; i++)
            {
                if ((int)PhotonNetwork.CurrentRoom.CustomProperties["" + i] == PhotonNetwork.LocalPlayer.ActorNumber && !removed)
                {
                    h["" + i] = 0;
                    removed = true;
                }

                if (studentPositions[i].GetComponent<ChairRef>().id == newSeatId && !seatfound)
                {
                    newSeatIndex = i;
                    seatfound = true;
                }

                if (removed && seatfound)
                {
                    Sit(newSeatIndex, 1);
                    return;
                }
            }          
        }

        //This is called when a player leaves the room, so we can free the student's place
        public override void OnPlayerLeftRoom(Player otherPlayer) {
            //get the seat number of plaer that left the room, and update room properties with the free seat (value to 0)
            if (PhotonNetwork.IsMasterClient) {
                for (int i = 0; i < studentPositions.Length; i++) {
                    if ((int)PhotonNetwork.CurrentRoom.CustomProperties["" + i] == otherPlayer.ActorNumber) {
                        h["" + i] = 0;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(h);
                        Debug.Log("User " + otherPlayer.ActorNumber + " left room, freeing up seat " + i);
                        return;
                    }
                }
            }
        }

        //If the new master client is this client, get a copy of the room properties
        public override void OnMasterClientSwitched(Player newMasterClient) {
            base.OnMasterClientSwitched(newMasterClient);
            if (newMasterClient.ActorNumber == actorNum) {
                h = PhotonNetwork.CurrentRoom.CustomProperties;
            }
        }

        //Class to load scenes async
        void GoToScene(int n) {
            returning = true;
            StartCoroutine(LoadScene(n));
        }

        IEnumerator LoadScene(int n) {
            yield return new WaitForSeconds(0.5f);

            AsyncOperation async = SceneManager.LoadSceneAsync(n);
            async.allowSceneActivation = false;

            yield return new WaitForSeconds(1);
            async.allowSceneActivation = true;
            if (n == 0) //if going back to menu destroy instance
            {
                Destroy(gameObject);
            }
        }

        public void Leave()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            string sceneName = "";

            switch (mode)
            {
                case Mode.Teacher:
                    sceneName = "LobbyVR";
                    break;
                case Mode.StudentPC:
                    sceneName = "LobbyPC";
                    break;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
