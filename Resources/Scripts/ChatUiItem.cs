using UnityEngine;
using TMPro;

public class ChatUiItem : MonoBehaviour
{
    public TextMeshProUGUI chatText;
    public TextMeshProUGUI nameText;

    public void SetItem(string chat, string _name)
    {
        chatText.text = chat;
        nameText.text = _name;
    }
}
