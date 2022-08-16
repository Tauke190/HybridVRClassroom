using UnityEngine;
using TMPro;
using EDVR.HybirdClassoom;

public class ChatUi : MonoBehaviour
{
    public GameObject chatPanel;
    public GameObject chatItemPrefab;
    public Transform parentTransform;
    public TextMeshProUGUI unreadMessageCountText;

    [Header("VR")]
    public GameObject chatPanelVr;
    public GameObject chatItemPrefabVr;
    public Transform parentTransformVr;
    public TextMeshProUGUI unreadMessageCountTextVr;

    private int unredMessageCount;

    private ChatRecordData recordData;
    private ChatRecordData allChatData;
    private ChatRecordData initializationData;

    private bool recording;

    private bool initialize;

    private void Start()
    {
        unreadMessageCountText.text = "";
        unreadMessageCountTextVr.text = "";

        if (!initialize)
        {
            Initialize();
        }

        UiEvents.Instance.OnRecord += Record;
    }

    private void Initialize()
    {
        allChatData = new ChatRecordData();

        initialize = true;
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

        recordData = new ChatRecordData();
        initializationData = allChatData;

        if (recording)
        {
            initializationData = allChatData;
        }
        else
        {
            SaveRecording();
        }
    }

    private void SaveRecording()
    {
        SaveSystem.SavePlayer(ref recordData, UID.ChatData);
        SaveSystem.SavePlayer(ref initializationData, UID.ChatInitialization);
    }

    public void SetChat(string chat, string _name, int actorid)
    {
        if (!initialize)
        {
            Initialize();
        }

        if (PlatformManager.instance.mode == PlatformManager.Mode.Teacher)
        {
            GameObject go = Instantiate(chatItemPrefabVr, parentTransformVr);
            go.GetComponent<ChatUiItem>().SetItem(chat, _name);

            if (!chatPanelVr.activeInHierarchy)
            {
                unredMessageCount++;
                unreadMessageCountTextVr.text = unredMessageCount.ToString();
            }
        }
        else
        {
            GameObject go = Instantiate(chatItemPrefab, parentTransform);
            go.GetComponent<ChatUiItem>().SetItem(chat, _name);

            if (!chatPanel.activeInHierarchy)
            {
                unredMessageCount++;
                unreadMessageCountText.text = unredMessageCount.ToString();
            }
        }

        allChatData.Add(chat, actorid);

        if (recording)
        {
            recordData.Add(chat, actorid);
        }
    }

    public void ResetUnreadMessage()
    {
        unredMessageCount = 0;
        unreadMessageCountText.text = "";
        unreadMessageCountTextVr.text = "";
    }

    private void Update()
    {
        if (chatPanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
        {
            UiEvents.Instance.SendChat();
        }
    }
}
