using TMPro;
using UnityEngine;
using Photon.Pun;

public class RoomNameSetter : MonoBehaviour
{
    private void Start()
    {
        if (GetComponent<TextMeshPro>())
        {
            GetComponent<TextMeshPro>().text = PhotonNetwork.CurrentRoom.Name;
        }

        if (GetComponent<TextMeshProUGUI>())
        {
            GetComponent<TextMeshProUGUI>().text = PhotonNetwork.CurrentRoom.Name;
        }
    }
}
