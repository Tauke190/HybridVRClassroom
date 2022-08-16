using Photon.Pun;
using EDVR.HybirdClassoom;
using TMPro;

public class PlayerChat : MonoBehaviourPun
{
    public TextMeshPro chatText;

    private void Start()
    {
        if (photonView.IsMine)
        {
            UiEvents.Instance.OnSendChat += SendChat;
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            UiEvents.Instance.OnSendChat -= SendChat;
        }
    }

    private void SendChat(string chat)
    {
        photonView.RPC("RpcSendChat", RpcTarget.AllBuffered, chat);
    }

    [PunRPC]
    private void RpcSendChat(string chat)
    {
        if (chatText)
        {
            chatText.text = chat;

            Invoke("ResetChatText", 2f);
        }

        string _name = GetComponent<NameDisplay>().PlayerName;

        if (GetComponent<FollowVRRig>())
        {
            _name += "(Host)";
        }

        FindObjectOfType<ChatUi>().SetChat(chat, _name, GetComponent<NameDisplay>().ActorId);
    }

    private void ResetChatText()
    {
        chatText.text = "";
    }
}
