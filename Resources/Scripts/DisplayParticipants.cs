using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using EDVR.HybirdClassoom;

public class DisplayParticipants : MonoBehaviourPunCallbacks
{
    public GameObject participantUiItem;

    public Transform uiParent;
    public Sprite speakerOnSprite;
    public Sprite speakerOffSprite;

    public override void OnEnable()
    {
        RefreshParticipantsList();
    }

    public void RefreshParticipantsList()
    {
        ClearAllPeople();

        foreach (var item in FindObjectsOfType<NameDisplay>())
        {
            GameObject go = Instantiate(participantUiItem, uiParent);

            string playerName = item.PlayerName;

            if (item.GetComponent<FollowVRRig>())
            {
                playerName += "(Host)";
            }

            go.GetComponent<ParticipantUiItem>().SetItem(playerName, item);
        }
    }

    private void ClearAllPeople()
    {
        List<GameObject> items = new List<GameObject>();

        for (int i = 0; i < uiParent.childCount; i++)
        {
            items.Add(uiParent.GetChild(i).gameObject);
        }

        while(items.Count != 0)
        {
            GameObject go = items[0];
            items.Remove(go);
            Destroy(go);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        RefreshParticipantsList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        RefreshParticipantsList();
    }
}
