using Photon.Pun;
using System.Collections.Generic;

public class SeeThroughSync : MonoBehaviourPun
{
    public List<VRToggleButton> buttons;

    private void Start()
    {
        FindObjectOfType<XRayToggler>().ToggleXRay(true);
    }

    public void Sync(VRToggleButton button)
    {
        photonView.RPC("RpcSync", RpcTarget.All, buttons.IndexOf(button));
    }

    [PunRPC]
    private void RpcSync(int index)
    {
        buttons[index].Toggle();
    }
}
