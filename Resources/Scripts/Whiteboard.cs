using System.Linq;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace EDVR.HybirdClassoom {
    public class Whiteboard : MonoBehaviourPunCallbacks
    {
        public GameObject whiteboardObject;
        public GameObject slides;

        private int textureSize = 2048;
        private Texture2D texture;
        private Color[] whitePixels;
        new Renderer renderer;

        private Dictionary<int, MarkerData> markerIDs = new Dictionary<int, MarkerData>();

        private 

        class MarkerData {
            public Color[] color;
            public bool touchingLastFrame;
            public float[] pos;
            public int pensize;
            public float[] colorRGB;
            public float lastX;
            public float lastY;
        }

        [SerializeField] List<Renderer> otherWhiteboards;

        bool applyingTexture;

        [HideInInspector] public PhotonView pv;

        private bool recording;

        private WhiteboardRecordData recordList;
        private WhiteboardRecordData allboarddata;
        private WhiteboardRecordData initializationData;
        //private Dictionary<float, Vector3> positionRecordList = new Dictionary<float, Vector3>();
        //private Dictionary<float, Vector3> rotationRecordList = new Dictionary<float, Vector3>();
        private TransformRecordData transformRecordData;

        public bool isReplay;

        private float Timer;
        private int positionIndex;
        private int drawingIndex;

        void Start()
        {
            if (isReplay)
            {
                recordList = SaveSystem.GetWhiteboardData();
                initializationData = SaveSystem.GetWhiteboardData();

                for (int i = 0; i < initializationData.id.Count; i++)
                {
                    DrawAtPosition(initializationData.id[i],
                        initializationData.pos[i],
                        initializationData.pensize[i],
                        initializationData.color[i]);
                }

                return;
            }

            renderer = GetComponent<Renderer>();
            texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Trilinear;
            texture.anisoLevel = 3;
            renderer.material.mainTexture = texture;

            foreach (var item in otherWhiteboards) {
                item.material.mainTexture = texture;
            }

            texture.Apply();

            pv = GetComponent<PhotonView>();

            whitePixels = Enumerable.Repeat(Color.white, textureSize * textureSize).ToArray();

            if (UiEvents.Instance)
            {
                UiEvents.Instance.OnRecord += Record;
            }

            recordList = new WhiteboardRecordData();
            allboarddata = new WhiteboardRecordData();
            initializationData = new WhiteboardRecordData();
            transformRecordData = new TransformRecordData();
        }

        private void OnDestroy()
        {
            if (recording)
            {
                SaveRecording();
            }

            if (UiEvents.Instance)
            {
                UiEvents.Instance.OnRecord -= Record;
            }
        }

        private void Record(bool isRecording)
        {
            recording = isRecording;

            recordList = new WhiteboardRecordData();
            transformRecordData = new TransformRecordData();

            if (recording)
            {
                initializationData = allboarddata;
                RecordTransform();
            }
            else
            {
                SaveRecording();
            }
        }

        private void SaveRecording()
        {
            SaveSystem.SavePlayer(ref recordList, UID.WhiteboardData);
            SaveSystem.SavePlayer(ref initializationData, UID.WhiteboardInitialization);
            SaveSystem.SavePlayer(ref transformRecordData, UID.WhiteboardTransformData);
        }

        //RPC sent by the Marker class so every user gets the information to draw in whiteboard.
        [PunRPC]
        public void DrawAtPosition(int id, float[] _pos, int _pensize, float[] _color)
        {
            //WhiteboardRecordData data = new WhiteboardRecordData(id, _pos, _pensize, _color);
            if (recording)
            {
                recordList.Add(id, _pos, _pensize, _color);
            }

            allboarddata.Add(id, _pos, _pensize, _color);

            if (!markerIDs.ContainsKey(id)) {
                markerIDs.Add(id, new MarkerData { touchingLastFrame = false, pos = _pos, pensize = _pensize, colorRGB = _color});;
            } else {
                markerIDs[id].pos = _pos;
            }

            markerIDs[id].color = SetColor(new Color(_color[0], _color[1], _color[2]), id);

            int x = (int)(markerIDs[id].pos[0] * textureSize - markerIDs[id].pensize/2);
            int y = (int)(markerIDs[id].pos[1] * textureSize - markerIDs[id].pensize / 2);

            //If last frame was not touching a marker, we don't need to lerp from last pixel coordinate to new, so we set the last coordinates to the new.
            if (!markerIDs[id].touchingLastFrame) {
                markerIDs[id].lastX = (float)x;
                markerIDs[id].lastY = (float)y;
                markerIDs[id].touchingLastFrame = true;
            }

            if (markerIDs[id].touchingLastFrame) {
                texture.SetPixels(x, y, markerIDs[id].pensize, markerIDs[id].pensize, markerIDs[id].color);

                //Lerp last pixel to new pixel, so we draw a continuous line.
                for (float t = 0.01f; t < 1.00f; t += 0.1f) {
                    int lerpX = (int)Mathf.Lerp(markerIDs[id].lastX, (float)x, t);
                    int lerpY = (int)Mathf.Lerp(markerIDs[id].lastY, (float)y, t);
                    texture.SetPixels(lerpX, lerpY, markerIDs[id].pensize, markerIDs[id].pensize, markerIDs[id].color);
                }

                //so it runs once per frame even if multiple markers are touching the whiteboard
                if (!applyingTexture) {
                    applyingTexture = true;
                    ApplyTexture();
                }
            }

            markerIDs[id].lastX = (float)x;
            markerIDs[id].lastY = (float)y;
        }

        public void ApplyTexture() {
            texture.Apply();
            applyingTexture = false;
        }

        [PunRPC]
        public void ResetTouch(int id) {
            if(markerIDs.ContainsKey(id))
                markerIDs[id].touchingLastFrame = false;
        }

        //Creates the color array for the marker id
        public Color[] SetColor(Color color, int id) {
            return Enumerable.Repeat(color, markerIDs[id].pensize * markerIDs[id].pensize).ToArray();
        }

        //To clear the whiteboard.
        public void ClearWhiteboard() {
            pv.RPC("RPC_ClearWhiteboard", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void RPC_ClearWhiteboard() {
            texture.SetPixels(whitePixels);
            texture.Apply();
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.OpCleanRpcBuffer(photonView);
            }
            recordList = new WhiteboardRecordData();
            allboarddata = new WhiteboardRecordData();
            initializationData = new WhiteboardRecordData();
        }

        public void RecordPosition()
        {
            if (recording)
            {
                RecordTransform();
            }
        }

        private void RecordTransform()
        {
            //positionRecordList.Add(RecordManager.Instance.RecordTimer, transform.position);
            //rotationRecordList.Add(RecordManager.Instance.RecordTimer, transform.eulerAngles);

            transformRecordData.Add(transform.position, transform.eulerAngles);
        }

        private void Update()
        {
            if (isReplay)
            {
                Timer += Time.deltaTime;
                if (positionIndex < recordList.id.Count && Timer > recordList.time[positionIndex])
                {
                    DrawAtPosition(recordList.id[positionIndex],
                        recordList.pos[positionIndex],
                        recordList.pensize[positionIndex],
                        recordList.color[positionIndex]);

                    slides.SetActive(false);
                    whiteboardObject.SetActive(true);

                    positionIndex++;
                }
            }
        }
    }
}
