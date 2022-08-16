using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace EDVR.HybirdClassoom {
    //This script is attached to the VR body, to ensure each part is following the correct tracker. This is done only if the body is owned by the player
    //and replicated around the network with the Photon Transform View component
    public class StudentBodyFollow : MonoBehaviourPunCallbacks
    {
        public GameObject body;

        PhotonView pv;

        public int ChairIndex { get; private set; }

        private bool initialized;

        private RecordPeople playerRecorder;

        private void Awake() {
            pv = GetComponent<PhotonView>();
        }

        private void Start()
        {
            if (pv.IsMine)
            {
                body.SetActive(false);
            }

            playerRecorder = FindObjectOfType<RecordPeople>();
        }

        public void SyncSeat(int _chairIndex, int isMoved)
        {
            ChairIndex = _chairIndex;

            pv.RPC("RpcSyncSeat", RpcTarget.Others, ChairIndex, isMoved);
        }

        public void GoToStage()
        {
            pv.RPC("RpcGoToStage", RpcTarget.Others);
        }

        [PunRPC]
        private void RpcGoToStage()
        {
            body.transform.position = PlatformManager.instance.stageSpawnPoint.position;
            body.transform.rotation = PlatformManager.instance.stageSpawnPoint.rotation;

            PlatformManager.instance.OnPlayerEnterStage(this);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (pv.IsMine && initialized && newPlayer != PhotonNetwork.LocalPlayer)
            {
                pv.RPC("RpcSyncSeat", newPlayer, ChairIndex, 0);
            }
        }

        [PunRPC]
        private void RpcSyncSeat(int _chairIndex, int moved)
        {
            Transform chair = PlatformManager.instance.studentPositions[_chairIndex];

            if (moved != -1)
            {
                ChairIndex = _chairIndex;
            }

            body.transform.SetParent(chair);

            body.transform.localPosition = new Vector3(0, 0.5f, 0.1f);
            body.transform.localEulerAngles = new Vector3(-90, 0, 0);

            body.transform.SetParent(transform);

            initialized = true;

            if (moved == -1)
            {
                PlatformManager.instance.OnPlayerLeftStage();
                return;
            }

            if (moved == 1)
            {
                playerRecorder.PlayerMove(GetComponent<NameDisplay>().ActorId, GetComponent<NameDisplay>().PlayerName, ChairIndex);
            }
            else
            {
                playerRecorder.PlayerEnter(GetComponent<NameDisplay>().ActorId, GetComponent<NameDisplay>().PlayerName, ChairIndex);
            }
        }
    }
}
