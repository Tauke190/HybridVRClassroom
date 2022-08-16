using UnityEngine;
using Photon.Pun;
using TMPro;
using EDVR.HybirdClassoom;

public class NameDisplay : MonoBehaviourPun
{
    public GameObject hand;
    public TextMeshPro nameText;

    public SpriteRenderer emojiSpriteRenderer;
    public Sprite[] emojiSprites;

    private bool client;

    public string PlayerName { get; private set; }
    public int ActorId { get; private set; }

    private RecordPeople peopleRecorder;

    private void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RpcSetName", RpcTarget.AllBuffered, PlayerPrefs.GetString("LocalPlayerName"), PhotonNetwork.LocalPlayer.ActorNumber);
        }

        if (GetComponent<FollowVRRig>() == null)
        {
            hand.SetActive(false);

            if (photonView.IsMine)
            {
                UiEvents.Instance.OnRaiseHand += RaiseHand;
            }

            emojiSpriteRenderer.sprite = null;

            client = true;
        }

        peopleRecorder = FindObjectOfType<RecordPeople>();
    }

    public void InviteThisPlayerOnState()
    {
        photonView.RPC("StageInvitation", RpcTarget.All);
    }

    [PunRPC]
    private void StageInvitation()
    {
        if (photonView.IsMine)
        {
            PlatformManager.instance.invitationPanel.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine && client)
        {
            UiEvents.Instance.OnRaiseHand -= RaiseHand;
        }
    }

    [PunRPC]
    private void RpcSetName(string _name, int actorid)
    {
        PlayerName = _name;

        if (nameText)
        {
            nameText.text = _name;
        }

        ActorId = actorid;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !hand.activeInHierarchy && photonView.IsMine && GetComponent<FollowVRRig>() == null)
        {
            RaiseHand();
        }
    }

    private void RaiseHand()
    {
        photonView.RPC("RpcRaiseHand", RpcTarget.All);
        hand.SetActive(true);
    }

    [PunRPC]
    private void RpcRaiseHand()
    {
        hand.SetActive(true);
        Invoke("DisableHand", 2f);

        peopleRecorder.HandRaise(ActorId);
    }

    private void DisableHand()
    {
        hand.SetActive(false);
    }

    public void SyncEmoji(int index)
    {
        photonView.RPC("RpcSyncEmoji", RpcTarget.All, index);
    }

    [PunRPC]
    private void RpcSyncEmoji(int index)
    {
        emojiSpriteRenderer.enabled = true;

        emojiSpriteRenderer.sprite = emojiSprites[index];

        Invoke("DeleteEmoji", 2f);

        peopleRecorder.Emoji(ActorId, index);
    }

    private void DeleteEmoji()
    {
        emojiSpriteRenderer.sprite = null;
    }
}
