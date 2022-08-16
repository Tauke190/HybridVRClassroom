using UnityEngine;

public class RecordingPlayer : MonoBehaviour
{
    public GameObject slides;
    public GameObject whiteboard;
    public Avatar player;
    public Transform marker;
    public Transform smallWhiteboard;

    public float Timer { get; private set; }

    private TransformRecordData headPositionData;
    private TransformRecordData hand1PositionData;
    private TransformRecordData hand2PositionData;
    private TransformRecordData markerPositionData;
    private TransformRecordData whiteboardPositionData;
    private PdfRecordData slideRecordData;

    private int boardIndex;
    private int headIndex;
    private int hand1Index;
    private int hand2Index;
    private int markerIndex;
    private int whiteboardIndex;
    private int slideIndex;

    private Transform headTransform;
    private Transform hand1Transform;
    private Transform hand2Transform;

    private void Start()
    {
        headPositionData = SaveSystem.GetTransformRecordData(UID.HeadPositionData);
        hand1PositionData = SaveSystem.GetTransformRecordData(UID.Hand1PositionData);
        hand2PositionData = SaveSystem.GetTransformRecordData(UID.Hand2PositionData);
        whiteboardPositionData = SaveSystem.GetTransformRecordData(UID.WhiteboardTransformData);
        markerPositionData = SaveSystem.GetTransformRecordData(UID.MarkerPositionData);
        slideRecordData = SaveSystem.GetSlideData();

        headTransform = new GameObject().transform;
        hand1Transform = new GameObject().transform;
        hand2Transform = new GameObject().transform;

        player.head.vrTarget = headTransform;
        player.leftHand.vrTarget = hand1Transform;
        player.rightHand.vrTarget = hand2Transform;
    }

    private void SetSlide()
    {
        if (slideIndex >= slideRecordData.texture.Count)
        {
            return;
        }

        slides.SetActive(true);
        whiteboard.SetActive(false);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(slideRecordData.texture[slideIndex]);
        slides.GetComponent<MeshRenderer>().material.mainTexture = tex;
    }

    private void SetTransform(Transform objtoset, TransformRecordData data, int index)
    {
        objtoset.transform.position = new Vector3(data.positionX[index], data.positionY[index], data.positionZ[index]);
        objtoset.transform.eulerAngles = new Vector3(data.rotationX[index], data.rotationY[index], data.rotationZ[index]);
    }

    private void Update()
    {
        Timer += Time.deltaTime;

        if (slideIndex < slideRecordData.texture.Count && Timer > slideRecordData.time[boardIndex])
        {
            SetSlide();
            slideIndex++;
        }

        if (headIndex < headPositionData.positionX.Count && Timer > headPositionData.time[headIndex])
        {
            SetTransform(headTransform, headPositionData, headIndex);
            headIndex++;
        }

        if (hand1Index < hand1PositionData.positionX.Count && Timer > hand1PositionData.time[hand1Index])
        {
            SetTransform(hand1Transform, hand1PositionData, hand1Index);
            hand1Index++;
        }

        if (hand2Index < hand2PositionData.positionX.Count && Timer > hand2PositionData.time[hand2Index])
        {
            SetTransform(hand2Transform, hand2PositionData, hand2Index);
            hand2Index++;
        }

        if (markerIndex < markerPositionData.positionX.Count && Timer > markerPositionData.time[markerIndex])
        {
            SetTransform(marker, markerPositionData, markerIndex);
            markerIndex++;
        }

        if (whiteboardIndex < whiteboardPositionData.positionX.Count && Timer > whiteboardPositionData.time[whiteboardIndex])
        {
            SetTransform(whiteboard.transform, whiteboardPositionData, whiteboardIndex);
            whiteboardIndex++;
        }
    }
}
