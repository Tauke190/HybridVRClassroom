using UnityEngine;
using EDVR.HybirdClassoom;

public class SendEmoji : MonoBehaviour
{
    public void Send(int index)
    {
        PlatformManager.instance.StudentPhoneBody.GetComponent<NameDisplay>().SyncEmoji(index);
    }
}
