using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BoneRef : MonoBehaviourPunCallbacks
{
    public List<NetworkInteractObject> Bones { get; private set; }

    public void SyncPickUp(NetworkInteractObject bone)
    {
        photonView.RPC("RpcSyncPickUp", RpcTarget.All, Bones.IndexOf(bone));
    }

    public override void OnEnable()
    {
        Bones = new List<NetworkInteractObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<NetworkInteractObject>() != null)
            {
                Bones.Add(transform.GetChild(i).GetComponent<NetworkInteractObject>());
            }
        }
    }

    [PunRPC]
    private void RpcSyncPickUp(int index)
    {
        if (index > 0 && index < Bones.Count)
        {
            Bones[index].RpcPickUp();
        }
        else
        {
            Debug.Log(index + " is invalid index");
        }
    }

    public void SyncDrop(NetworkInteractObject bone, bool placed)
    {
        photonView.RPC("RpcSyncDrop", RpcTarget.All, Bones.IndexOf(bone), placed ? 1 : 0);
    }

    [PunRPC]
    private void RpcSyncDrop(int index, int placed)
    {
        if (index > 0 && index < Bones.Count)
        {
            Bones[index].RpcDrop(placed == 1);
        }
        else
        {
            Debug.Log(index + " is invalid index");
        }
    }

    public void Freeze(bool value)
    {
        foreach (var item in Bones)
        {
            item.Rb.isKinematic = value || item.Placed;
        }
    }

    public void SyncTargetPosition()
    {
        foreach (var item in Bones)
        {
            photonView.RPC("ReceiveTargetPosition", RpcTarget.Others, Bones.IndexOf(item), item.Target.transform.position.x, item.Target.transform.position.y, item.Target.transform.position.z);
            photonView.RPC("ReceiveTargetRotation", RpcTarget.Others, Bones.IndexOf(item), item.Target.transform.eulerAngles.x, item.Target.transform.eulerAngles.y, item.Target.transform.eulerAngles.z);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient && newPlayer != PhotonNetwork.LocalPlayer)
        {
            foreach (var item in Bones)
            {
                photonView.RPC("ReceiveTargetPosition", newPlayer, Bones.IndexOf(item), item.Target.transform.position.x, item.Target.transform.position.y, item.Target.transform.position.z);
                photonView.RPC("ReceiveTargetRotation", newPlayer, Bones.IndexOf(item), item.Target.transform.eulerAngles.x, item.Target.transform.eulerAngles.y, item.Target.transform.eulerAngles.z);
            }
        }
    }

    [PunRPC]
    private void ReceiveTargetPosition(int index, float x, float y, float z)
    {
        Bones[index].SetTargetPosition(x, y, z);
    }

    [PunRPC]
    private void ReceiveTargetRotation(int index, float x, float y, float z)
    {
        Bones[index].SetTargetRotation(x, y, z);
    }
}
