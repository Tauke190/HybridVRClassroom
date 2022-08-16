using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using EDVR.HybirdClassoom;

public class RecordPeople : MonoBehaviourPunCallbacks
{
    private bool recording;

    private PeopleData recordList;
    private PeopleData initializationData;
    private IndexRecordData handraiseData;
    private TwoIndexRecordData emojiData;

    private void Start()
    {
        UiEvents.Instance.OnRecord += Record;

        recordList = new PeopleData();
        initializationData = new PeopleData();
        handraiseData = new IndexRecordData();
        emojiData = new TwoIndexRecordData();
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
            foreach (var item in FindObjectsOfType<StudentBodyFollow>())
            {
                NameDisplay nameClass = item.GetComponent<NameDisplay>();
                initializationData.Add(nameClass.ActorId, nameClass.PlayerName, item.ChairIndex, PeopleData.State.Spawned);
            }
        }
        else
        {
            SaveRecording();
        }
    }

    private void SaveRecording()
    {
        SaveSystem.SavePlayer(ref recordList, UID.PeopleData);
        SaveSystem.SavePlayer(ref initializationData, UID.PeopleInitializationData);
        SaveSystem.SavePlayer(ref handraiseData, UID.HandRaiseData);
        SaveSystem.SavePlayer(ref emojiData, UID.EmojiData);

        recordList = new PeopleData();
        initializationData = new PeopleData();
        handraiseData = new IndexRecordData();
        emojiData = new TwoIndexRecordData();
    }

    public void PlayerEnter(int actorid, string playername, int chair)
    {
        if (!recording)
        {
            return;
        }

        recordList.Add(actorid, playername, chair, PeopleData.State.Spawned);
    }

    public void PlayerMove(int actorid, string playername, int chair)
    {
        if (!recording)
        {
            return;
        }

        recordList.Add(actorid, playername, chair, PeopleData.State.Moved);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (!recording)
        {
            return;
        }

        recordList.Add(otherPlayer.ActorNumber, otherPlayer.NickName, 0, PeopleData.State.Left);
    }

    public void HandRaise(int id)
    {
        if (!recording)
        {
            return;
        }

        handraiseData.Add(id);
    }

    public void Emoji(int id, int emojiIndex)
    {
        if (!recording)
        {
            return;
        }

        emojiData.Add(id, emojiIndex);
    }
}
