using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParticipantUiItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    private NameDisplay playerRef;

    public void SetItem(string _name, NameDisplay _playerRef)
    {
        nameText.text = _name;
        playerRef = _playerRef;

        if (GetComponentInChildren<Button>())
        {
            GetComponentInChildren<Button>().onClick.AddListener(Invite);
        }
    }

    private void Invite()
    {
        VRPointer.Instance.ToggleOffVRUI();
        playerRef.InviteThisPlayerOnState();
    }
}
