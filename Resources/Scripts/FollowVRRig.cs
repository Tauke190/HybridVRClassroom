using UnityEngine;
using Photon.Pun;

namespace EDVR.HybirdClassoom {
    //This script is attached to the VR body, to ensure each part is following the correct tracker. This is done only if the body is owned by the player
    //and replicated around the network with the Photon Transform View component
    public class FollowVRRig : MonoBehaviour {
        public Transform[] body;
        [SerializeField] SkinnedMeshRenderer lHand;
        [SerializeField] SkinnedMeshRenderer rHand;


        PhotonView pv;

        private bool recording;

        private TransformRecordData headData;
        private TransformRecordData hand1Data;
        private TransformRecordData hand2Data;

        private float timer;
        private float checkPoint;

        private void Awake() {
            pv = GetComponent<PhotonView>();
            SetAvatarFollow();

            if (PlatformManager.instance.mode == PlatformManager.Mode.Teacher) {
                EnableRenderers();
            }
        }

        private void Start()
        {
            UiEvents.Instance.OnRecord += Record;

            headData = new TransformRecordData();
            hand1Data = new TransformRecordData();
            hand2Data = new TransformRecordData();
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
                timer = 0f;
                checkPoint = RecordManager.Instance.DeltaTime();
            }
            else
            {
                SaveRecording();
            }
        }

        void EnableRenderers() {
            lHand.enabled = true;
            rHand.enabled = true;
        }

        [PunRPC]
        public void SetAvatarFollow() {
            PlatformManager.instance.avatar.head.vrTarget = body[0];
            PlatformManager.instance.avatar.leftHand.vrTarget = body[1];
            PlatformManager.instance.avatar.rightHand.vrTarget = body[2];
        }

        void Update()
        {
            if (pv.IsMine)
            {
                for (int i = 0; i < body.Length; i++)
                {
                    body[i].position = PlatformManager.instance.teacherRigParts[i].position;
                    body[i].rotation = PlatformManager.instance.teacherRigParts[i].rotation;
                }
            }
        }

        private void FixedUpdate()
        {
            if (recording)
            {
                timer += Time.deltaTime;
                if (timer >= checkPoint)
                {
                    headData.Add(body[0].transform.position, body[0].transform.eulerAngles);
                    hand1Data.Add(body[1].transform.position, body[1].transform.eulerAngles);
                    hand2Data.Add(body[2].transform.position, body[2].transform.eulerAngles);
                    checkPoint += RecordManager.Instance.DeltaTime();
                }
            }
        }

        private void SaveRecording()
        {
            SaveSystem.SavePlayer(ref headData, UID.HeadPositionData);
            SaveSystem.SavePlayer(ref hand1Data, UID.Hand1PositionData);
            SaveSystem.SavePlayer(ref hand2Data, UID.Hand2PositionData);

            headData = new TransformRecordData();
            hand1Data = new TransformRecordData();
            hand2Data = new TransformRecordData();
        }
    }
}
